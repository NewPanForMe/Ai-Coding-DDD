using Client.Domain.Abstractions;

namespace Client.Domain.DomainEvents;

/// <summary>
/// 订单取消领域事件，在订单被取消后触发。
/// </summary>
public sealed class OrderCancelledDomainEvent : IDomainEvent
{
    /// <summary>
    /// 初始化订单取消事件。
    /// </summary>
    /// <param name="orderId">订单 ID。</param>
    public OrderCancelledDomainEvent(Guid orderId)
    {
        OrderId = orderId;
        OccurredOn = DateTime.UtcNow;
    }

    /// <summary>
    /// 获取订单 ID。
    /// </summary>
    public Guid OrderId { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; }
}
