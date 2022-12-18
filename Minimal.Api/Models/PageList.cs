namespace Minimal.Api.Models;

public class PageList<T> where T : class
{
    public PageList() { }

    public PageList(IEnumerable<T> items, int total, int page, int pageSize)
    {
        Items = items;
        Total = total;
        Page = page;
        PageSize = pageSize;
    }

    public IEnumerable<T> Items { get; init; }
    public int Total { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }

}