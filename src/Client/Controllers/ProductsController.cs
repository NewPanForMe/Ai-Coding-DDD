using Client.Application.DTOs;
using Client.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

/// <summary>
/// 产品管理 API 控制器。
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductAppService _productService;
    private readonly ILogger<ProductsController> _logger;

    /// <summary>
    /// 初始化产品控制器。
    /// </summary>
    /// <param name="productService">产品应用服务。</param>
    /// <param name="logger">日志记录器。</param>
    public ProductsController(IProductAppService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// 分页查询产品列表。
    /// </summary>
    /// <param name="pageIndex">页码（1 based）。</param>
    /// <param name="pageSize">每页数量。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>分页产品列表。</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductInfo>>> GetPagedList(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetPagedListAsync(pageIndex, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// 根据 ID 获取产品。
    /// </summary>
    /// <param name="id">产品 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>产品信息；未找到返回 404。</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductInfo>> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("获取产品: {ProductId}", id);
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product is null) return NotFound();
        return Ok(product);
    }

    /// <summary>
    /// 创建新产品。
    /// </summary>
    /// <param name="dto">产品信息。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>创建的产品信息。</returns>
    [HttpPost]
    public async Task<ActionResult<ProductInfo>> Create([FromBody] ProductInfo dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("创建产品: {ProductName}", dto.Name);
        var created = await _productService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// 更新产品信息。
    /// </summary>
    /// <param name="id">产品 ID。</param>
    /// <param name="dto">产品信息。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的产品信息；未找到返回 404。</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductInfo>> Update(Guid id, [FromBody] ProductInfo dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("更新产品: {ProductId}", id);
        var updated = await _productService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    /// <summary>
    /// 删除产品。
    /// </summary>
    /// <param name="id">产品 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>成功返回 204；未找到返回 404。</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("删除产品: {ProductId}", id);
        var deleted = await _productService.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
