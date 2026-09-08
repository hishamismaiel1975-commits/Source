using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Platform.Infrastructure.Persistence.EFCore.Extensions;

/// <summary>
/// Provides dynamic Entity Framework Core include support.
/// Converts a property-chain expression such as
/// <c>x => x.Companies.Persons.Bed</c> into an EF Core
/// <c>Include().ThenInclude().ThenInclude()</c> chain.
/// 
/// Supports both reference and collection navigation properties.
/// </summary>
public static class EfIncludeExtensions
{
    public static IQueryable<T> IncludePath<T>(
        this IQueryable<T> query,
        Expression<Func<T, object>> expression)
        where T : class
    {
        var properties = GetPropertyChain(expression);

        if (properties.Count == 0)
            return query;

        // --------------------------------------------------
        // Include(...)
        // --------------------------------------------------

        object current = ApplyInclude(
            query,
            typeof(T),
            properties[0]);

        // --------------------------------------------------
        // ThenInclude(...)
        // --------------------------------------------------

        for (int i = 1; i < properties.Count; i++)
        {
            var previousProperty = properties[i - 1];
            var currentProperty = properties[i];

            current = ApplyThenInclude(
                current,
                typeof(T),
                previousProperty,
                currentProperty);
        }

        return (IQueryable<T>)current;
    }

    // ======================================================
    // Include
    // ======================================================

    private static object ApplyInclude(
        IQueryable query,
        Type entityType,
        PropertyInfo property)
    {
        var includeMethod = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(x =>
                x.Name == nameof(
                    EntityFrameworkQueryableExtensions.Include))
            .Where(x => x.IsGenericMethodDefinition)
            .Where(x => x.GetGenericArguments().Length == 2)
            .Single(x =>
            {
                var parameters = x.GetParameters();

                if (parameters.Length != 2)
                    return false;

                var secondParameter = parameters[1].ParameterType;

                return secondParameter.IsGenericType &&
                       secondParameter.GetGenericTypeDefinition() ==
                       typeof(Expression<>);
            });

        var propertyType = property.PropertyType;

        var genericMethod = includeMethod.MakeGenericMethod(
            entityType,
            propertyType);

        var lambda = CreatePropertyLambda(
            entityType,
            property);

        return genericMethod.Invoke(
            null,
            new object[]
            {
                query,
                lambda
            })!;
    }

    // ======================================================
    // ThenInclude
    // ======================================================

    private static object ApplyThenInclude(
        object query,
        Type entityType,
        PropertyInfo previousProperty,
        PropertyInfo currentProperty)
    {
        var previousType = previousProperty.PropertyType;

        if (IsCollection(previousType))
        {
            return ApplyCollectionThenInclude(
                query,
                entityType,
                previousType,
                currentProperty);
        }

        return ApplyReferenceThenInclude(
            query,
            entityType,
            previousType,
            currentProperty);
    }

    // ======================================================
    // Collection -> ...
    // ======================================================

    private static object ApplyCollectionThenInclude(
        object query,
        Type entityType,
        Type collectionType,
        PropertyInfo currentProperty)
    {
        var elementType =
            GetCollectionElementType(collectionType)
            ?? throw new InvalidOperationException(
                $"Could not determine collection element type for '{collectionType}'.");

        var navigationType = currentProperty.PropertyType;

        var method = GetThenIncludeMethod(
            collection: true);

        var genericMethod = method.MakeGenericMethod(
            entityType,
            elementType,
            navigationType);

        var lambda = CreatePropertyLambda(
            elementType,
            currentProperty);

        return genericMethod.Invoke(
            null,
            new[]
            {
                query,
                lambda
            })!;
    }

    // ======================================================
    // Reference -> ...
    // ======================================================

    private static object ApplyReferenceThenInclude(
        object query,
        Type entityType,
        Type previousType,
        PropertyInfo currentProperty)
    {
        var navigationType = currentProperty.PropertyType;

        var method = GetThenIncludeMethod(
            collection: false);

        var genericMethod = method.MakeGenericMethod(
            entityType,
            previousType,
            navigationType);

        var lambda = CreatePropertyLambda(
            previousType,
            currentProperty);

        return genericMethod.Invoke(
            null,
            new[]
            {
                query,
                lambda
            })!;
    }

    // ======================================================
    // Find correct ThenInclude overload
    // ======================================================

    private static MethodInfo GetThenIncludeMethod(
        bool collection)
    {
        var methods = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(x =>
                x.Name == nameof(
                    EntityFrameworkQueryableExtensions.ThenInclude))
            .Where(x => x.IsGenericMethodDefinition)
            .Where(x => x.GetGenericArguments().Length == 3)
            .Where(x => x.GetParameters().Length == 2);

        foreach (var method in methods)
        {
            var sourceType =
                method.GetParameters()[0].ParameterType;

            if (!sourceType.IsGenericType)
                continue;

            if (sourceType.GetGenericTypeDefinition() !=
                typeof(IIncludableQueryable<,>))
                continue;

            var propertyType =
                sourceType.GetGenericArguments()[1];

            var isCollection =
                propertyType.IsGenericType &&
                propertyType.GetGenericTypeDefinition() ==
                typeof(IEnumerable<>);

            if (isCollection == collection)
                return method;
        }

        throw new InvalidOperationException(
            $"Could not find EF Core ThenInclude overload. " +
            $"Collection: {collection}");
    }

    // ======================================================
    // Expression creation
    // ======================================================

    private static LambdaExpression CreatePropertyLambda(
        Type parameterType,
        PropertyInfo property)
    {
        var parameter =
            Expression.Parameter(
                parameterType,
                "x");

        var body =
            Expression.Property(
                parameter,
                property);

        var delegateType =
            typeof(Func<,>).MakeGenericType(
                parameterType,
                property.PropertyType);

        return Expression.Lambda(
            delegateType,
            body,
            parameter);
    }

    // ======================================================
    // Extract:
    //
    // x => x.Companies.Persons.Bed
    //
    // into:
    //
    // Companies
    // Persons
    // Bed
    // ======================================================

    private static List<PropertyInfo> GetPropertyChain<T>(
        Expression<Func<T, object>> expression)
    {
        var properties = new List<PropertyInfo>();

        Expression? current = expression.Body;

        // Remove boxing:
        //
        // x => (object)x.Companies
        //
        if (current is UnaryExpression unary &&
            unary.NodeType == ExpressionType.Convert)
        {
            current = unary.Operand;
        }

        while (current is MemberExpression member)
        {
            if (member.Member is not PropertyInfo property)
            {
                throw new ArgumentException(
                    "Include expression can contain only properties.",
                    nameof(expression));
            }

            properties.Add(property);

            current = member.Expression;
        }

        if (current is not ParameterExpression)
        {
            throw new ArgumentException(
                "Invalid include expression.",
                nameof(expression));
        }

        properties.Reverse();

        return properties;
    }

    // ======================================================
    // Collection helpers
    // ======================================================

    private static bool IsCollection(Type type)
    {
        if (type == typeof(string))
            return false;

        return typeof(IEnumerable)
            .IsAssignableFrom(type);
    }

    private static Type? GetCollectionElementType(Type type)
    {
        if (type.IsArray)
            return type.GetElementType();

        if (type.IsGenericType)
        {
            var genericDefinition =
                type.GetGenericTypeDefinition();

            if (genericDefinition == typeof(IEnumerable<>))
            {
                return type.GetGenericArguments()[0];
            }
        }

        var enumerableInterface =
            type.GetInterfaces()
                .FirstOrDefault(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() ==
                    typeof(IEnumerable<>));

        return enumerableInterface?
            .GetGenericArguments()[0];
    }
}