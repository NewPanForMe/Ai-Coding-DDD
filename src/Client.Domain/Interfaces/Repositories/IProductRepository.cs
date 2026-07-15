using Client.Domain.Abstractions;
using Client.Domain.Entities;

namespace Client.Domain.Interfaces.Repositories;

/// <summary>
/// 产品仓储接口，定义产品聚合根的持久化操作。
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// 根据 SKU 查找产品。
    /// </summary>
    /// <param name="sku">产品 SKU 编码。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>找到则返回产品实例，否则返回 null。</returns>
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
}
