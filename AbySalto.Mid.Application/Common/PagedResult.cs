namespace AbySalto.Mid.Application.Common;

public sealed class PagedResult<T>
{
    public PagedResult()
    {
    }

    public PagedResult(int page, int pageSize, int totalCount, int totalPages, IReadOnlyList<T> items)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalPages;
        Items = items;
    }

    public int Page { get; init; }
    public int PageSize { get; init; }

    public int TotalCount { get; init; }
    public int TotalPages { get; init; }

    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
}