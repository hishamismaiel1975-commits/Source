using System.Linq.Expressions;

namespace Platform.Core.Extensions;

public static class ExpressionExtensions
{
    public static void AddIf<T>(
        this ICollection<Expression<Func<T, bool>>> filters,
        bool condition,
        Expression<Func<T, bool>> expression)
    {
        if (condition)
        {
            filters.Add(expression);
        }
    }
}
