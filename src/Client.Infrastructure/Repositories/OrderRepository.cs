using Client.Domain.Entities;
using Client.Domain.Interfaces.Repositories;
using Client.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Client.Infrastructure.Repositories;

/// <summary>
/// 订单仓储实现。
/// </summary>
public sealed class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    /// <summary>
    /// 初始化订单仓储。
    /// </summary>
    /// <param name="context">数据库上下文。</param>
    public OrderRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public override async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbSet.Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken)
            .ConfigureAwait(false);

    /// <inheritdoc />
    public override async Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedAsync(
        int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var totalCount = await _dbSet.AsNoTracking().CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await _dbSet.AsNoTracking()
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking()
            .Include(o => o.OrderItems)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}
