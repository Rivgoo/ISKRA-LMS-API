using System.Linq.Expressions;

namespace Iskra.Infrastructure.Shared.Search;

/// <summary>
/// Defines database specific text search logic.
/// </summary>
public interface ISearchProvider
{
    /// <summary>
    /// Applies text search criteria across multiple entity properties.
    /// </summary>
    /// <typeparam name="T">Target entity type.</typeparam>
    /// <param name="query">Original query source.</param>
    /// <param name="terms">Text to locate.</param>
    /// <param name="method">Logic for the comparison.</param>
    /// <param name="selectors">Lambda expressions for target properties.</param>
    /// <returns>A queryable with search filters applied.</returns>
    IQueryable<T> ApplySearch<T>(
        IQueryable<T> query,
        string terms,
        SearchMethod method,
        params Expression<Func<T, string?>>[] selectors);
}