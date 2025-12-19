using Iskra.Application.Errors;

namespace Iskra.Application.Filters.Abstractions.Errors;

/// <summary>
/// Provides error templates for query and filter operations.
/// </summary>
public static class QueryErrors
{
    public static Error InvalidSortField(string property) =>
        Error.BadRequest("Query.InvalidSortField", "Property '{0}' is not valid for sorting.", property);

    public static Error DuplicateSortField(string property) =>
        Error.BadRequest("Query.DuplicateSortField", "Sort criteria for '{0}' already exists.", property);

    public static Error InvalidInput =>
        Error.BadRequest("Query.InvalidInput", "The query parameters are malformed.");
}