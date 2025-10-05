namespace EV_SCMMS.Core.Application.Base;

/// <summary>
/// Paged result class for paginated data
/// </summary>
/// <typeparam name="T">Type of data items</typeparam>
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedResult()
    {
    }

    public PagedResult(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public static PagedResult<T> Create(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        return new PagedResult<T>(items, count, pageNumber, pageSize);
    }
}
