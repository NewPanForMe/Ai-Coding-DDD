using Client.Domain.Abstractions;
using Client.Domain.DomainEvents;
using Client.Domain.ValueObjects;

namespace Client.Domain.Entities;

/// <summary>
/// 订单聚合根，代表一笔客户订单。
/// </summary>
public sealed class Order : IAggregateRoot
{
    private readonly List<OrderItem> _orderItems = [];
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// 订单状态枚举。
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 未知状态。
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 待处理。
        /// </summary>
        Pending = 1,

        /// <summary>
        /// 已确认。
        /// </summary>
        Confirmed = 2,

        /// <summary>
        /// 已发货。
        /// </summary>
        Shipped = 3,

        /// <summary>
        /// 已完成。
        /// </summary>
        Completed = 4,

        /// <summary>
        /// 已取消。
        /// </summary>
        Cancelled = 5
    }

    /// <summary>
    /// 初始化订单。
    /// </summary>
    /// <param name="customerId">客户 ID。</param>
    /// <param name="customerName">客户姓名。</param>
    /// <param name="shippingAddress">收货地址。</param>
    /// <exception cref="ArgumentException">客户 ID 为空时抛出。</exception>
    public Order(Guid customerId, string customerName, Address shippingAddress)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("客户 ID 不能为空。", nameof(customerId));
        }

        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("客户姓名不能为空。", nameof(customerName));
        }

        Id = Guid.NewGuid();
        CustomerId = customerId;
        CustomerName = customerName;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 私有构造函数，供 EF Core 使用。
    /// </summary>
    private Order()
    {
        CustomerName = string.Empty;
        ShippingAddress = new Address("", "", "", "", "", "");
    }

    /// <summary>
    /// 获取订单唯一标识。
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// 获取客户 ID。
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// 获取客户姓名（快照）。
    /// </summary>
    public string CustomerName { get; private set; }

    /// <summary>
    /// 获取收货地址（快照）。
    /// </summary>
    public Address ShippingAddress { get; private set; }

    /// <summary>
    /// 获取订单状态。
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// 获取订单创建时间。
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 获取订单项集合。
    /// </summary>
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    /// <summary>
    /// 获取领域事件集合。
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// 添加订单项到订单。
    /// </summary>
    /// <param name="orderItem">订单项。</param>
    public void AddOrderItem(OrderItem orderItem)
    {
        _orderItems.Add(orderItem);
    }

    /// <summary>
    /// 计算订单总金额。
    /// </summary>
    /// <returns>所有订单项金额之和。</returns>
    public Money GetTotalAmount()
    {
        if (_orderItems.Count == 0)
        {
            return new Money(0);
        }

        var total = _orderItems[0].GetTotalPrice();
        for (var i = 1; i < _orderItems.Count; i++)
        {
            total = total.Add(_orderItems[i].GetTotalPrice());
        }

        return total;
    }

    /// <summary>
    /// 确认订单（状态从 Pending → Confirmed）。
    /// </summary>
    /// <exception cref="InvalidOperationException">订单非待处理状态时抛出。</exception>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException($"只有待处理的订单才能确认，当前状态: {Status}。");
        }

        Status = OrderStatus.Confirmed;
    }

    /// <summary>
    /// 取消订单（状态变更为 Cancelled）。
    /// </summary>
    /// <exception cref="InvalidOperationException">订单已完成或已取消时抛出。</exception>
    public void Cancel()
    {
        if (Status == OrderStatus.Completed)
        {
            throw new InvalidOperationException("已完成的订单不可取消。");
        }

        if (Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("订单已经被取消。");
        }

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledDomainEvent(Id));
    }

    /// <summary>
    /// 标记订单完成。
    /// </summary>
    /// <exception cref="InvalidOperationException">订单非已发货状态时抛出。</exception>
    public void Complete()
    {
        if (Status != OrderStatus.Shipped)
        {
            throw new InvalidOperationException($"只有已发货的订单才能完成，当前状态: {Status}。");
        }

        Status = OrderStatus.Completed;
    }

    /// <summary>
    /// 清除所有领域事件。
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// 添加领域事件。
    /// </summary>
    /// <param name="domainEvent">要添加的领域事件。</param>
    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
