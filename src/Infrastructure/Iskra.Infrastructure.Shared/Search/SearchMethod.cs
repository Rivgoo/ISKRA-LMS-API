namespace Iskra.Infrastructure.Shared.Search;

/// <summary>
/// Defines the behavior of text search comparisons.
/// </summary>
public enum SearchMethod
{
    /// <summary>
    /// Matches text containing the term.
    /// </summary>
    Contains,

    /// <summary>
    /// Matches text starting with the term.
    /// </summary>
    StartsWith,

    /// <summary>
    /// Matches text ending with the term.
    /// </summary>
    EndsWith,

    /// <summary>
    /// Matches the exact text.
    /// </summary>
    Equals
}