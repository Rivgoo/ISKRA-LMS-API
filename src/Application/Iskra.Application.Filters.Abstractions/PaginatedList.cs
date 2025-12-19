namespace Iskra.Application.Filters.Abstractions;

/// <summary>
/// Manages data segments and page info.
/// </summary>
/// <typeparam name="T">Result item type.</typeparam>
public class PaginatedList<T>
{
    /// <summary>
    /// Items in the current segment.
    /// </summary>
    public List<T> Items { get; }

    /// <summary>
    /// Current page index.
    /// </summary>
    public int PageIndex { get; }

    /// <summary>
    /// Maximum items in the segment.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Total records found.
    /// </summary>
    public int TotalCount { get; private set; }

    /// <summary>
    /// Initialize the result object.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="pageIndex">Index.</param>
    /// <param name="pageSize">Size.</param>
    public PaginatedList(List<T> items, int pageIndex, int pageSize)
    {
        Items = items;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    /// <summary>
    /// Total pages available.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Previous page existence.
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// Next page existence.
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;

    /// <summary>
    /// Set total records.
    /// </summary>
    /// <param name="count">Total count.</param>
    /// <returns>The result object.</returns>
    public PaginatedList<T> SetTotalCount(int count)
    {
        TotalCount = count;
        return this;
    }
}