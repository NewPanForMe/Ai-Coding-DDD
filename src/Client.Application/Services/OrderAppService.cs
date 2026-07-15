using Client.Application.DTOs;
using Client.Application.EventHandlers;
using Client.Application.Interfaces;
using Client.Domain.Abstractions;
using Client.Domain.DomainServices;
using Client.Domain.Entities;
using Client.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Client.Application.Services;

/// <summary>
/// 订单应用服务，负责订单查询、创建和取消操作的编排。
/// </summary>
public sealed class OrderAppService : IOrderAppService
{
    private readonly IOrderRepository _orderRepository;
    private readonly OrderDomainService _domainService;
    private readonly OrderCancelledDomainEventHandler _eventHandler;
    private readonly ILogger<OrderAppService> _logger;

    /// <summary>
    /// 初始化订单应用服务。
    /// </summary>
    /// <param name="orderRepository">订单仓储。</param>
    /// <param name="domainService">订单领域服务。</param>
    /// <param name="eventHandler">订单取消事件处理器。</param>
    /// <param name="logger">日志记录器。</param>
    public OrderAppService(
        IOrderRepository orderRepository,
        OrderDomainService domainService,
        OrderCancelledDomainEventHandler eventHandler,
        ILogger<OrderAppService> logger)
    {
        _orderRepository = orderRepository;
        _domainService = domainService;
        _eventHandler = eventHandler;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<PagedResult<OrderInfo>> GetPagedListAsync(
        int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _orderRepository.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        var orderInfos = items.Select(ToInfo).ToList();
        return new PagedResult<OrderInfo>(orderInfos, totalCount, pageIndex, pageSize);
    }

    /// <inheritdoc />
    public async Task<OrderInfo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        return order is null ? null : ToInfo(order);
    }

    /// <inheritdoc />
    public async Task<OrderInfo> CreateAsync(
        Guid customerId,
        IReadOnlyList<(Guid ProductId, int Quantity)> items,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("开始创建订单: 客户 {CustomerId}, 商品数 {ItemCount}", customerId, items.Count);

        var order = await _domainService.CreateOrderAsync(customerId, items, cancellationToken);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("订单 {OrderId} 持久化成功，总金额: {TotalAmount}", order.Id, order.GetTotalAmount().Amount);
        return ToInfo(order);
    }

    /// <inheritdoc />
    public async Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null)
        {
            _logger.LogWarning("订单取消失败: 订单 {OrderId} 不存在", id);
            return false;
        }

        order.Cancel();
        await _orderRepository.SaveChangesAsync(cancellationToken);

        // 分发领域事件
        DispatchDomainEvents(order);

        _logger.LogInformation("订单 {OrderId} 取消成功", id);
        return true;
    }

    /// <summary>
    /// 分发聚合根中的所有领域事件。
    /// </summary>
    /// <param name="order">订单聚合根。</param>
    private void DispatchDomainEvents(Order order)
    {
        foreach (var domainEvent in order.DomainEvents)
        {
            _eventHandler.Handle(domainEvent);
        }

        order.ClearDomainEvents();
    }

    /// <summary>
    /// 将订单实体转换为 DTO。
    /// </summary>
    /// <param name="order">订单实体。</param>
    /// <returns>订单信息 DTO。</returns>
    private static OrderInfo ToInfo(Order order)
    {
        return new OrderInfo
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            Status = order.Status.ToString(),
            TotalAmount = order.GetTotalAmount().Amount,
            CreatedAt = order.CreatedAt,
            Address = order.ShippingAddress is null ? null : new AddressInfo
            {
                Country = order.ShippingAddress.Country,
                Province = order.ShippingAddress.Province,
                City = order.ShippingAddress.City,
                District = order.ShippingAddress.District,
                Detail = order.ShippingAddress.Street,
                PostalCode = order.ShippingAddress.PostalCode
            },
            Items = order.OrderItems.Select(i => new OrderItemInfo
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Sku = i.Sku,
                UnitPrice = i.UnitPrice.Amount,
                Quantity = i.Quantity,
                SubTotal = i.GetTotalPrice().Amount
            }).ToList()
        };
    }
}
