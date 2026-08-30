namespace CodeWay.Application.Common;

/// <summary>
/// Generic paginated result wrapper returned by all list queries.
/// </summary>
/// <typeparam name="T">The DTO type for each item.</typeparam>
public sealed class PaginatedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public PaginatedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }

    public static PaginatedResult<T> Empty(int page, int pageSize) =>
        new([], 0, page, pageSize);
}
