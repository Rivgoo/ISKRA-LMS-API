namespace Iskra.Application.Filters.Abstractions;

/// <summary>
/// Represents a single sorting rule.
/// </summary>
/// <param name="Property">Name of the entity property.</param>
/// <param name="Direction">The direction of the sort.</param>
public record SortCriteria(string Property, SortDirection Direction);