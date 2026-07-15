using Client.Domain.Abstractions;
using Client.Domain.Entities;

namespace Client.Domain.Interfaces.Repositories;

/// <summary>
/// 客户仓储接口，定义客户聚合根的持久化操作。
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    /// <summary>
    /// 根据邮箱查找客户。
    /// </summary>
    /// <param name="email">客户邮箱。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>找到则返回客户实例，否则返回 null。</returns>
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
