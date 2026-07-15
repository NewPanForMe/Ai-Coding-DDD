using Client.Domain.Abstractions;

namespace Client.Domain.DomainEvents;

/// <summary>
/// 订单创建领域事件，在订单成功创建后触发。
/// </summary>
public sealed class OrderCreatedDomainEvent : IDomainEvent
{
    /// <summary>
    /// 初始化订单创建事件。
    /// </summary>
    /// <param name="orderId">订单 ID。</param>
    public OrderCreatedDomainEvent(Guid orderId)
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
