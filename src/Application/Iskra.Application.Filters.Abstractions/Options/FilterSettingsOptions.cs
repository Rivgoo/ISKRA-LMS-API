namespace Iskra.Application.Filters.Abstractions.Options;

/// <summary>
/// Defines limits for filtering and pagination.
/// </summary>
public class FilterSettingsOptions
{
    /// <summary>
    /// Default records per segment.
    /// </summary>
    public int DefaultPageSize { get; set; } = 20;

    /// <summary>
    /// Maximum allowed records per segment.
    /// </summary>
    public int MaxPageSize { get; set; } = 100;
}