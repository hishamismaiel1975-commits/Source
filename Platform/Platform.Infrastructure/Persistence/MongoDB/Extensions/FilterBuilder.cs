using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Platform.Infrastructure.Persistence.MongoDB.Extensions;

//Convert Expression<Func<T, bool>> to FilterDefinition<T> for MongoDB queries case-insensitive
public static class FilterBuilder
{
    public static FilterDefinition<T> Build<T>(
        Expression<Func<T, bool>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        return BuildExpression<T>(expression.Body);
    }

    private static FilterDefinition<T> BuildExpression<T>(
        Expression expression)
    {
        expression = RemoveConvert(expression);

        if (expression is MethodCallExpression methodCall)
        {
            return BuildMethodCall<T>(methodCall);
        }

        if (expression is BinaryExpression binary)
        {
            return BuildBinary<T>(binary);
        }

        if (expression is UnaryExpression unary &&
            unary.NodeType == ExpressionType.Not)
        {
            var filter =
                BuildExpression<T>(unary.Operand);

            return Builders<T>.Filter.Not(filter);
        }

        if (expression is MemberExpression member &&
            member.Type == typeof(bool))
        {
            return BuildBooleanMember<T>(member);
        }

        throw new NotSupportedException(
            $"Expression '{expression}' is not supported.");
    }

    // ============================================================
    // Binary
    // ============================================================

    private static FilterDefinition<T> BuildBinary<T>(
        BinaryExpression expression)
    {
        if (expression.NodeType == ExpressionType.AndAlso)
        {
            var left =
                BuildExpression<T>(expression.Left);

            var right =
                BuildExpression<T>(expression.Right);

            return Builders<T>.Filter.And(left, right);
        }

        if (expression.NodeType == ExpressionType.OrElse)
        {
            var left =
                BuildExpression<T>(expression.Left);

            var right =
                BuildExpression<T>(expression.Right);

            return Builders<T>.Filter.Or(left, right);
        }

        if (!TryGetMemberAndValue(
                expression,
                out var memberExpression,
                out var valueExpression))
        {
            throw new NotSupportedException(
                $"Binary expression '{expression}' is not supported.");
        }

        var fieldName =
            GetFieldName(memberExpression);

        var value =
            GetValue(valueExpression);

        return expression.NodeType switch
        {
            ExpressionType.Equal =>
                BuildEqual<T>(
                    fieldName,
                    memberExpression,
                    value),

            ExpressionType.NotEqual =>
                BuildNotEqual<T>(
                    fieldName,
                    memberExpression,
                    value),

            ExpressionType.GreaterThan =>
                BuildComparison<T>(
                    fieldName,
                    value,
                    "$gt"),

            ExpressionType.GreaterThanOrEqual =>
                BuildComparison<T>(
                    fieldName,
                    value,
                    "$gte"),

            ExpressionType.LessThan =>
                BuildComparison<T>(
                    fieldName,
                    value,
                    "$lt"),

            ExpressionType.LessThanOrEqual =>
                BuildComparison<T>(
                    fieldName,
                    value,
                    "$lte"),

            _ => throw new NotSupportedException(
                $"Operator '{expression.NodeType}' is not supported.")
        };
    }

    // ============================================================
    // Equal
    // ============================================================

    private static FilterDefinition<T> BuildEqual<T>(
        string fieldName,
        Expression memberExpression,
        object? value)
    {
        // Case-insensitive string equality
        if (memberExpression.Type == typeof(string))
        {
            if (value == null)
            {
                return CreateSimpleFilter<T>(
                    fieldName,
                    BsonNull.Value);
            }

            var pattern =
                "^" +
                Regex.Escape(value.ToString()!) +
                "$";

            return CreateRegexFilter<T>(
                fieldName,
                pattern);
        }

        return CreateOperatorFilter<T>(
            fieldName,
            "$eq",
            value);
    }

    // ============================================================
    // Not Equal
    // ============================================================

    private static FilterDefinition<T> BuildNotEqual<T>(
        string fieldName,
        Expression memberExpression,
        object? value)
    {
        if (memberExpression.Type == typeof(string))
        {
            if (value == null)
            {
                return CreateOperatorFilter<T>(
                    fieldName,
                    "$ne",
                    null);
            }

            var pattern =
                "^" +
                Regex.Escape(value.ToString()!) +
                "$";

            var regexFilter =
                CreateRegexFilter<T>(
                    fieldName,
                    pattern);

            return Builders<T>.Filter.Not(
                regexFilter);
        }

        return CreateOperatorFilter<T>(
            fieldName,
            "$ne",
            value);
    }

    // ============================================================
    // Comparison
    // ============================================================

    private static FilterDefinition<T> BuildComparison<T>(
        string fieldName,
        object? value,
        string operatorName)
    {
        if (value == null)
        {
            throw new NotSupportedException(
                $"Operator '{operatorName}' cannot be used with null.");
        }

        return CreateOperatorFilter<T>(
            fieldName,
            operatorName,
            value);
    }

    // ============================================================
    // Create BSON operator filter
    //
    // Example:
    //
    // Price > 100
    //
    // becomes:
    //
    // {
    //     "Price": {
    //         "$gt": 100
    //     }
    // }
    // ============================================================

    private static FilterDefinition<T> CreateOperatorFilter<T>(
        string fieldName,
        string operatorName,
        object? value)
    {
        BsonValue bsonValue;

        if (value == null)
        {
            bsonValue = BsonNull.Value;
        }
        else
        {
            bsonValue = BsonValue.Create(value);
        }

        var document =
            new BsonDocument
            {
                {
                    fieldName,
                    new BsonDocument
                    {
                        {
                            operatorName,
                            bsonValue
                        }
                    }
                }
            };

        return new BsonDocumentFilterDefinition<T>(
            document);
    }

    // ============================================================
    // Simple filter
    //
    // Example:
    //
    // {
    //     "Name": null
    // }
    // ============================================================

    private static FilterDefinition<T> CreateSimpleFilter<T>(
        string fieldName,
        BsonValue value)
    {
        var document =
            new BsonDocument
            {
                {
                    fieldName,
                    value
                }
            };

        return new BsonDocumentFilterDefinition<T>(
            document);
    }

    // ============================================================
    // Regex
    // ============================================================

    private static FilterDefinition<T> CreateRegexFilter<T>(
        string fieldName,
        string pattern)
    {
        var document =
            new BsonDocument
            {
                {
                    fieldName,
                    new BsonDocument
                    {
                        {
                            "$regex",
                            pattern
                        },
                        {
                            "$options",
                            "i"
                        }
                    }
                }
            };

        return new BsonDocumentFilterDefinition<T>(
            document);
    }

    // ============================================================
    // String methods
    // ============================================================

    private static FilterDefinition<T> BuildMethodCall<T>(
        MethodCallExpression expression)
    {
        if (expression.Method.DeclaringType == typeof(string))
        {
            return expression.Method.Name switch
            {
                nameof(string.Contains) =>
                    BuildContains<T>(expression),

                nameof(string.StartsWith) =>
                    BuildStartsWith<T>(expression),

                nameof(string.EndsWith) =>
                    BuildEndsWith<T>(expression),

                _ => throw new NotSupportedException(
                    $"String method '{expression.Method.Name}' is not supported.")
            };
        }

        if (expression.Method.Name ==
            nameof(Enumerable.Contains))
        {
            return BuildEnumerableContains<T>(
                expression);
        }

        throw new NotSupportedException(
            $"Method '{expression.Method.Name}' is not supported.");
    }

    // ============================================================
    // Contains
    //
    // x.Name.Contains(value)
    // ============================================================

    private static FilterDefinition<T> BuildContains<T>(
        MethodCallExpression expression)
    {
        if (expression.Object == null)
        {
            throw new NotSupportedException(
                "String.Contains must have an object.");
        }

        var fieldName =
            GetFieldName(
                expression.Object);

        var value =
            GetValue(
                expression.Arguments[0]);

        if (value == null)
        {
            return Builders<T>.Filter.Empty;
        }

        var pattern =
            Regex.Escape(
                value.ToString()!);

        return CreateRegexFilter<T>(
            fieldName,
            pattern);
    }

    // ============================================================
    // StartsWith
    // ============================================================

    private static FilterDefinition<T> BuildStartsWith<T>(
        MethodCallExpression expression)
    {
        if (expression.Object == null)
        {
            throw new NotSupportedException(
                "String.StartsWith must have an object.");
        }

        var fieldName =
            GetFieldName(
                expression.Object);

        var value =
            GetValue(
                expression.Arguments[0]);

        if (value == null)
        {
            return Builders<T>.Filter.Empty;
        }

        var pattern =
            "^" +
            Regex.Escape(
                value.ToString()!);

        return CreateRegexFilter<T>(
            fieldName,
            pattern);
    }

    // ============================================================
    // EndsWith
    // ============================================================

    private static FilterDefinition<T> BuildEndsWith<T>(
        MethodCallExpression expression)
    {
        if (expression.Object == null)
        {
            throw new NotSupportedException(
                "String.EndsWith must have an object.");
        }

        var fieldName =
            GetFieldName(
                expression.Object);

        var value =
            GetValue(
                expression.Arguments[0]);

        if (value == null)
        {
            return Builders<T>.Filter.Empty;
        }

        var pattern =
            Regex.Escape(
                value.ToString()!) +
            "$";

        return CreateRegexFilter<T>(
            fieldName,
            pattern);
    }

    // ============================================================
    // Enumerable.Contains
    //
    // ids.Contains(x.Id)
    //
    // Generates:
    //
    // {
    //     "Id": {
    //         "$in": [...]
    //     }
    // }
    // ============================================================

    private static FilterDefinition<T>
        BuildEnumerableContains<T>(
            MethodCallExpression expression)
    {
        if (expression.Arguments.Count != 2)
        {
            throw new NotSupportedException(
                "Contains expression has an invalid number of arguments.");
        }

        var collectionExpression =
            expression.Arguments[0];

        var memberExpression =
            RemoveConvert(
                expression.Arguments[1]);

        var fieldName =
            GetFieldName(
                memberExpression);

        var collection =
            GetValue(
                collectionExpression);

        if (collection is not IEnumerable enumerable)
        {
            throw new NotSupportedException(
                "The first argument of Contains must be a collection.");
        }

        var bsonArray =
            new BsonArray();

        foreach (var item in enumerable)
        {
            bsonArray.Add(
                item == null
                    ? BsonNull.Value
                    : BsonValue.Create(item));
        }

        var document =
            new BsonDocument
            {
                {
                    fieldName,
                    new BsonDocument
                    {
                        {
                            "$in",
                            bsonArray
                        }
                    }
                }
            };

        return new BsonDocumentFilterDefinition<T>(
            document);
    }

    // ============================================================
    // Boolean property
    //
    // x.IsActive
    // ============================================================

    private static FilterDefinition<T>
        BuildBooleanMember<T>(
            MemberExpression expression)
    {
        var fieldName =
            GetFieldName(expression);

        return CreateOperatorFilter<T>(
            fieldName,
            "$eq",
            true);
    }

    // ============================================================
    // Find member/value
    // ============================================================

    private static bool TryGetMemberAndValue(
        BinaryExpression expression,
        out Expression member,
        out Expression value)
    {
        var left =
            RemoveConvert(
                expression.Left);

        var right =
            RemoveConvert(
                expression.Right);

        if (left is MemberExpression)
        {
            member = left;
            value = right;

            return true;
        }

        if (right is MemberExpression)
        {
            member = right;
            value = left;

            return true;
        }

        member = null!;
        value = null!;

        return false;
    }

    // ============================================================
    // Get Mongo field name
    //
    // x.Name
    //
    // returns:
    //
    // Name
    // ============================================================

    private static string GetFieldName(
        Expression expression)
    {
        expression =
            RemoveConvert(expression);

        if (expression is not MemberExpression member)
        {
            throw new NotSupportedException(
                $"'{expression}' is not a member expression.");
        }

        return GetMemberPath(member);
    }

    private static string GetMemberPath(
        MemberExpression member)
    {
        var parts =
            new Stack<string>();

        Expression? current =
            member;

        while (current is MemberExpression currentMember)
        {
            parts.Push(
                currentMember.Member.Name);

            current =
                currentMember.Expression;
        }

        return string.Join(
            ".",
            parts);
    }

    // ============================================================
    // Get runtime value
    //
    // Supports:
    //
    // request.ProductName
    // request.BrandId
    // local variables
    // constants
    // ============================================================

    private static object? GetValue(
        Expression expression)
    {
        expression =
            RemoveConvert(expression);

        if (expression is ConstantExpression constant)
        {
            return constant.Value;
        }

        var converted =
            Expression.Convert(
                expression,
                typeof(object));

        var lambda =
            Expression.Lambda<Func<object?>>(
                converted);

        return lambda.Compile()();
    }

    // ============================================================
    // Remove Convert
    // ============================================================

    private static Expression RemoveConvert(
        Expression expression)
    {
        while (expression.NodeType == ExpressionType.Convert ||
               expression.NodeType == ExpressionType.ConvertChecked)
        {
            expression =
                ((UnaryExpression)expression).Operand;
        }

        return expression;
    }
}