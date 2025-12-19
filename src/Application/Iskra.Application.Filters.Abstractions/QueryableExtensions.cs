using System.Linq.Expressions;
using System.Reflection;

namespace Iskra.Application.Filters.Abstractions;

public static class QueryableExtensions
{
    public static IOrderedQueryable<T> ApplySort<T>(this IQueryable<T> source, string property, SortDirection direction)
    {
        var method = direction switch
        {
            SortDirection.Asc => "OrderBy",
            SortDirection.Desc => "OrderByDescending",
            _ => "OrderBy"
        };

        return BuildOrderExpression(source, property, method);
    }

    private static IOrderedQueryable<T> BuildOrderExpression<T>(IQueryable<T> source, string propertyPath, string methodName)
    {
        var type = typeof(T);
        var parameter = Expression.Parameter(type, "x");
        Expression expression = parameter;

        foreach (var part in propertyPath.Split('.'))
        {
            var info = type.GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) ?? 
                throw new ArgumentException("Property not found");
			expression = Expression.Property(expression, info);
            type = info.PropertyType;
        }

        var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
        var lambda = Expression.Lambda(delegateType, expression, parameter);

        var queryMethod = typeof(Queryable).GetMethods().Single(m =>
            m.Name == methodName &&
            m.IsGenericMethodDefinition &&
            m.GetParameters().Length == 2);

        var result = queryMethod.MakeGenericMethod(typeof(T), type).Invoke(null, [source, lambda]);
        return (IOrderedQueryable<T>)result!;
    }
}