using Client.Application.DTOs;

namespace Client.Application.Interfaces;

/// <summary>
/// 客户应用服务接口，定义客户管理的应用层操作。
/// </summary>
public interface ICustomerAppService
{
    /// <summary>
    /// 获取所有客户。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>客户列表。</returns>
    Task<IReadOnlyList<CustomerInfo>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据 ID 获取客户。
    /// </summary>
    /// <param name="id">客户 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>客户信息；未找到返回 null。</returns>
    Task<CustomerInfo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建新客户。
    /// </summary>
    /// <param name="dto">客户信息。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>创建的客户信息。</returns>
    Task<CustomerInfo> CreateAsync(CustomerInfo dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新客户信息。
    /// </summary>
    /// <param name="id">客户 ID。</param>
    /// <param name="dto">客户信息。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的客户信息；未找到返回 null。</returns>
    Task<CustomerInfo?> UpdateAsync(Guid id, CustomerInfo dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除客户。
    /// </summary>
    /// <param name="id">客户 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>成功返回 true；未找到返回 false。</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
