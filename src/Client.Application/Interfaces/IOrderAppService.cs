using Client.Application.DTOs;

namespace Client.Application.Interfaces;

/// <summary>
/// 订单应用服务接口，定义订单管理的应用层操作。
/// </summary>
public interface IOrderAppService
{
    /// <summary>
    /// 分页查询订单列表。
    /// </summary>
    /// <param name="pageIndex">页码（从 1 开始）。</param>
    /// <param name="pageSize">每页数量。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>分页订单列表。</returns>
    Task<PagedResult<OrderInfo>> GetPagedListAsync(
        int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据 ID 获取订单。
    /// </summary>
    /// <param name="id">订单 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>订单信息；未找到返回 null。</returns>
    Task<OrderInfo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建新订单。
    /// </summary>
    /// <param name="customerId">下单客户 ID。</param>
    /// <param name="orderItems">订单项列表（产品 ID + 数量）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>创建的订单信息。</returns>
    Task<OrderInfo> CreateAsync(
        Guid customerId,
        IReadOnlyList<(Guid ProductId, int Quantity)> orderItems,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消订单。
    /// </summary>
    /// <param name="id">订单 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>成功返回 true；未找到返回 false。</returns>
    Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
