using Client.Domain.Entities;
using Client.Domain.Interfaces.Repositories;
using Client.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Client.Infrastructure.Repositories;

/// <summary>
/// 客户仓储实现。
/// </summary>
public sealed class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    /// <summary>
    /// 初始化客户仓储。
    /// </summary>
    /// <param name="context">数据库上下文。</param>
    public CustomerRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Email == email, cancellationToken)
            .ConfigureAwait(false);
}
