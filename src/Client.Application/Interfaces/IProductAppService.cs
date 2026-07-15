using Client.Application.DTOs;

namespace Client.Application.Interfaces;

/// <summary>
/// 产品应用服务接口，定义产品管理的应用层操作。
/// </summary>
public interface IProductAppService
{
    /// <summary>
    /// 分页查询产品列表。
    /// </summary>
    /// <param name="pageIndex">页码（从 1 开始）。</param>
    /// <param name="pageSize">每页数量。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>分页产品列表。</returns>
    Task<PagedResult<ProductInfo>> GetPagedListAsync(
        int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据 ID 获取产品。
    /// </summary>
    /// <param name="id">产品 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>产品信息；未找到返回 null。</returns>
    Task<ProductInfo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建新产品。
    /// </summary>
    /// <param name="dto">产品信息。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>创建的产品信息。</returns>
    Task<ProductInfo> CreateAsync(ProductInfo dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新产品信息。
    /// </summary>
    /// <param name="id">产品 ID。</param>
    /// <param name="dto">产品信息。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的产品信息；未找到返回 null。</returns>
    Task<ProductInfo?> UpdateAsync(Guid id, ProductInfo dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除产品。
    /// </summary>
    /// <param name="id">产品 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>成功返回 true；未找到返回 false。</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
