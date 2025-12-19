using Iskra.Infrastructure.Shared.Search;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Iskra.Modules.MariaDb.Search;

/// <summary>
/// MariaDB implementation using native LIKE operations.
/// </summary>
internal sealed class MariaDbSearchProvider : ISearchProvider
{
    public IQueryable<T> ApplySearch<T>(
        IQueryable<T> query,
        string terms,
        SearchMethod method,
        params Expression<Func<T, string?>>[] selectors)
    {
        if (selectors.Length == 0 || string.IsNullOrWhiteSpace(terms))
            return query;

        string pattern = BuildPattern(terms, method);
        ParameterExpression unifiedParameter = Expression.Parameter(typeof(T), "e");
        ParameterUpdateVisitor visitor = new(unifiedParameter);

        Expression? combinedBody = null;
        MethodInfo likeMethod = typeof(DbFunctionsExtensions).GetMethod(
            nameof(DbFunctionsExtensions.Like),
            [typeof(DbFunctions), typeof(string), typeof(string)])!;

        foreach (Expression<Func<T, string?>> selector in selectors)
        {
            // Unify the parameter in the current selector body
            Expression propertyExpression = visitor.Visit(selector.Body);

            // Create EF.Functions.Like(property, pattern)
            MethodCallExpression likeCall = Expression.Call(
                null,
                likeMethod,
                Expression.Constant(EF.Functions),
                propertyExpression,
                Expression.Constant(pattern));

            combinedBody = combinedBody == null
                ? likeCall
                : Expression.OrElse(combinedBody, likeCall);
        }

        Expression<Func<T, bool>> finalLambda = Expression.Lambda<Func<T, bool>>(
            combinedBody!,
            unifiedParameter);

        return query.Where(finalLambda);
    }

    private static string BuildPattern(string terms, SearchMethod method)
    {
        return method switch
        {
            SearchMethod.StartsWith => $"{terms}%",
            SearchMethod.EndsWith => $"%{terms}",
            SearchMethod.Equals => terms,
            _ => $"%{terms}%"
        };
    }

    /// <summary>
    /// Replaces original lambda parameters with a single unified parameter.
    /// This prevents LINQ translation errors.
    /// </summary>
    private sealed class ParameterUpdateVisitor(ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) => target;
    }
}