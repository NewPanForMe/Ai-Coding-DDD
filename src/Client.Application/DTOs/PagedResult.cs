namespace Client.Application.DTOs;

/// <summary>
/// 通用分页结果。
/// </summary>
/// <typeparam name="T">数据项类型。</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>
    /// 初始化分页结果。
    /// </summary>
    /// <param name="items">当前页的数据项集合。</param>
    /// <param name="totalCount">总记录数。</param>
    /// <param name="pageIndex">当前页码（从 1 开始）。</param>
    /// <param name="pageSize">每页数量。</param>
    public PagedResult(IReadOnlyList<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    /// <summary>
    /// 获取当前页数据项。
    /// </summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>
    /// 获取总记录数。
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// 获取当前页码。
    /// </summary>
    public int PageIndex { get; }

    /// <summary>
    /// 获取每页数量。
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// 获取总页数。
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// 是否有上一页。
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// 是否有下一页。
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;
}
