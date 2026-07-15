using System.Linq.Expressions;
using Client.Domain.Abstractions;
using Client.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Client.Infrastructure.Repositories;

/// <summary>
/// 通用仓储基类，提供基于 EF Core 的 CRUD 实现。
/// </summary>
/// <typeparam name="T">聚合根类型。</typeparam>
public abstract class BaseRepository<T> : IRepository<T> where T : class, IAggregateRoot
{
    /// <summary>
    /// DbContext 实例。
    /// </summary>
    protected readonly AppDbContext _context;

    /// <summary>
    /// 当前实体的 DbSet。
    /// </summary>
    protected readonly DbSet<T> _dbSet;

    /// <summary>
    /// 初始化仓储基类。
    /// </summary>
    /// <param name="context">数据库上下文。</param>
    protected BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbSet.FindAsync([id], cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var totalCount = await _dbSet.AsNoTracking().CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await _dbSet.AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await _dbSet.AddAsync(entity, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    /// <inheritdoc />
    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    /// <inheritdoc />
    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
}
