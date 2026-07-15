using System.Linq.Expressions;

namespace Client.Domain.Abstractions;

/// <summary>
/// 通用仓储接口，定义聚合根的持久化操作契约。
/// </summary>
/// <typeparam name="T">聚合根类型，必须实现 IAggregateRoot 接口。</typeparam>
public interface IRepository<T> where T : class, IAggregateRoot
{
    /// <summary>
    /// 根据主键获取实体。
    /// </summary>
    /// <param name="id">实体主键。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>找到则返回实体实例，否则返回 null。</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有实体。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>实体集合。</returns>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据规约条件查询实体。
    /// </summary>
    /// <param name="predicate">查询条件。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>符合条件的实体集合。</returns>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页查询实体。
    /// </summary>
    /// <param name="pageIndex">页码（从 1 开始）。</param>
    /// <param name="pageSize">每页数量。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>分页结果。</returns>
    Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加实体到仓储。
    /// </summary>
    /// <param name="entity">要添加的实体。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新实体。
    /// </summary>
    /// <param name="entity">要更新的实体。</param>
    void Update(T entity);

    /// <summary>
    /// 删除实体。
    /// </summary>
    /// <param name="entity">要删除的实体。</param>
    void Remove(T entity);

    /// <summary>
    /// 保存所有变更。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>受影响的行数。</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
