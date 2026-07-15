using Client.Application.DTOs;
using Client.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

/// <summary>
/// 客户管理 API 控制器。
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerAppService _customerService;
    private readonly ILogger<CustomersController> _logger;

    /// <summary>
    /// 初始化客户控制器。
    /// </summary>
    /// <param name="customerService">客户应用服务。</param>
    /// <param name="logger">日志记录器。</param>
    public CustomersController(ICustomerAppService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有客户。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>客户列表。</returns>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerInfo>>> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("获取所有客户");
        var customers = await _customerService.GetAllAsync(cancellationToken);
        return Ok(customers);
    }

    /// <summary>
    /// 根据 ID 获取客户。
    /// </summary>
    /// <param name="id">客户 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>客户信息；未找到返回 404。</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerInfo>> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("获取客户: {CustomerId}", id);
        var customer = await _customerService.GetByIdAsync(id, cancellationToken);
        if (customer is null) return NotFound();
        return Ok(customer);
    }

    /// <summary>
    /// 创建新客户。
    /// </summary>
    /// <param name="dto">客户信息。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>创建的客户信息。</returns>
    [HttpPost]
    public async Task<ActionResult<CustomerInfo>> Create([FromBody] CustomerInfo dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("创建客户: {CustomerName}", dto.Name);
        var created = await _customerService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// 更新客户信息。
    /// </summary>
    /// <param name="id">客户 ID。</param>
    /// <param name="dto">客户信息。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的客户信息；未找到返回 404。</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerInfo>> Update(Guid id, [FromBody] CustomerInfo dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("更新客户: {CustomerId}", id);
        var updated = await _customerService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    /// <summary>
    /// 删除客户。
    /// </summary>
    /// <param name="id">客户 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>成功返回 204；未找到返回 404。</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("删除客户: {CustomerId}", id);
        var deleted = await _customerService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
