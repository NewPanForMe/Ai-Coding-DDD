using Client.Domain.Abstractions;
using Client.Domain.Entities;

namespace Client.Domain.Interfaces.Repositories;

/// <summary>
/// 订单仓储接口，定义订单聚合根的持久化操作。
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// 根据客户 ID 查询订单。
    /// </summary>
    /// <param name="customerId">客户 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>该客户的所有订单。</returns>
    Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
