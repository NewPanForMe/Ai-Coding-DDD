using Client.Domain.Entities;
using Client.Domain.Interfaces.Repositories;
using Client.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Client.Infrastructure.Repositories;

/// <summary>
/// 产品仓储实现。
/// </summary>
public sealed class ProductRepository : BaseRepository<Product>, IProductRepository
{
    /// <summary>
    /// 初始化产品仓储。
    /// </summary>
    /// <param name="context">数据库上下文。</param>
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().FirstOrDefaultAsync(p => p.Sku == sku, cancellationToken)
            .ConfigureAwait(false);
}
