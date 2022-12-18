using System.Reflection;

namespace Minimal.Api.Models;

public class PagingData
{
    /// <summary>
    /// Default value = 1
    /// </summary>
    public int Page { get; init; } = 1;

    /// <summary>
    /// Default value = 10
    /// </summary>
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Example: `id asc, code desc`
    /// </summary>
    public string? SortBy { get; init; }

    public static ValueTask<PagingData?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        const string pageKey = "page";
        const string pageSizeKey = "pageSize";
        const string sortByKey = "sortBy";

        int.TryParse(context.Request.Query[pageKey], out var page);
        page = page == 0 ? 1 : page;

        int.TryParse(context.Request.Query[pageSizeKey], out var pageSize);
        pageSize = pageSize == 0 ? 10 : pageSize;

        var result = new PagingData
        {
            Page = page,
            PageSize = pageSize,
            SortBy = context.Request.Query[sortByKey],
        };

        return ValueTask.FromResult<PagingData?>(result);
    }
}