using Iskra.Application.Results;

namespace Iskra.Application.Filters.Abstractions;

/// <summary>
/// Defines basic query parameters for client requests.
/// </summary>
public interface IQueryRequest
{
    /// <summary>
    /// Current page index.
    /// </summary>
    int PageIndex { get; set; }

    /// <summary>
    /// Returns the list of active sorting rules.
    /// </summary>
    IReadOnlyList<SortCriteria> GetSorts();

    /// <summary>
    /// Registers a new sorting rule for the request.
    /// </summary>
    Result AddSort(string property, SortDirection direction);
}