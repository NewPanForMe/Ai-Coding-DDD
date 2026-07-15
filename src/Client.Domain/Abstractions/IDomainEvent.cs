namespace Client.Domain.Abstractions;

/// <summary>
/// 领域事件基接口，所有领域事件必须实现此接口。
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// 获取事件发生的 UTC 时间。
    /// </summary>
    DateTime OccurredOn { get; }
}
