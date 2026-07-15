using Client.Application.DTOs;
using Client.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

/// <summary>
/// 订单管理 API 控制器。
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderAppService _orderService;
    private readonly ILogger<OrdersController> _logger;

    /// <summary>
    /// 初始化订单控制器。
    /// </summary>
    /// <param name="orderService">订单应用服务。</param>
    /// <param name="logger">日志记录器。</param>
    public OrdersController(IOrderAppService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// 分页查询订单列表。
    /// </summary>
    /// <param name="pageIndex">页码（1 based）。</param>
    /// <param name="pageSize">每页数量。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>分页订单列表。</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderInfo>>> GetPagedList(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _orderService.GetPagedListAsync(pageIndex, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// 根据 ID 获取订单。
    /// </summary>
    /// <param name="id">订单 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>订单信息；未找到返回 404。</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderInfo>> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("获取订单: {OrderId}", id);
        var order = await _orderService.GetByIdAsync(id, cancellationToken);
        if (order is null) return NotFound();
        return Ok(order);
    }

    /// <summary>
    /// 创建新订单。
    /// </summary>
    /// <param name="request">创建订单请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>创建的订单信息。</returns>
    [HttpPost]
    public async Task<ActionResult<OrderInfo>> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("创建订单: 客户 {CustomerId}", request.CustomerId);

        var items = request.Items
            .Select(i => (i.ProductId, i.Quantity))
            .ToList()
            .AsReadOnly();

        var order = await _orderService.CreateAsync(request.CustomerId, items, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// 取消订单。
    /// </summary>
    /// <param name="id">订单 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>成功返回 204；未找到返回 404。</returns>
    [HttpDelete("{id:guid}/cancel")]
    public async Task<ActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("取消订单: {OrderId}", id);
        var cancelled = await _orderService.CancelAsync(id, cancellationToken);
        if (!cancelled) return NotFound();
        return NoContent();
    }
}

/// <summary>
/// 创建订单的请求模型。
/// </summary>
public sealed class CreateOrderRequest
{
    /// <summary>
    /// 下单客户 ID。
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// 订单项列表。
    /// </summary>
    public List<OrderItemRequest> Items { get; set; } = new();
}

/// <summary>
/// 订单项的请求模型。
/// </summary>
public sealed class OrderItemRequest
{
    /// <summary>
    /// 产品 ID。
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 订购数量。
    /// </summary>
    public int Quantity { get; set; }
}
