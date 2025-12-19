using Iskra.Application.Filters.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Iskra.Api.Abstractions.Filters;

/// <summary>
/// Wraps raw sorting arrays for unified processing.
/// </summary>
/// <param name="Fields">Properties to sort by.</param>
/// <param name="Directions">Directions for each property.</param>
public record SortParams(
    [FromQuery] string[]? Fields,
    [FromQuery] SortDirection[]? Directions);