using Client.Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace Client.Application.EventHandlers;

/// <summary>
/// 订单取消领域事件处理器。
/// </summary>
public sealed class OrderCancelledDomainEventHandler
{
    private readonly ILogger<OrderCancelledDomainEventHandler> _logger;

    /// <summary>
    /// 初始化订单取消事件处理器。
    /// </summary>
    /// <param name="logger">日志记录器。</param>
    public OrderCancelledDomainEventHandler(ILogger<OrderCancelledDomainEventHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 处理领域事件。
    /// </summary>
    /// <param name="domainEvent">领域事件。</param>
    public void Handle(IDomainEvent domainEvent)
    {
        _logger.LogInformation("处理领域事件: {EventType}", domainEvent.GetType().Name);
    }
}
