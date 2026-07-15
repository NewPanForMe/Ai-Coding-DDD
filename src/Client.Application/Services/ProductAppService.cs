using Client.Application.DTOs;
using Client.Application.Interfaces;
using Client.Domain.Entities;
using Client.Domain.ValueObjects;
using Client.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Client.Application.Services;

/// <summary>
/// 产品应用服务，负责产品 CRUD 操作的编排。
/// </summary>
public sealed class ProductAppService : IProductAppService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductAppService> _logger;

    /// <summary>
    /// 初始化产品应用服务。
    /// </summary>
    /// <param name="productRepository">产品仓储。</param>
    /// <param name="logger">日志记录器。</param>
    public ProductAppService(IProductRepository productRepository, ILogger<ProductAppService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<PagedResult<ProductInfo>> GetPagedListAsync(
        int pageIndex = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _productRepository.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        var productInfos = items.Select(ToInfo).ToList();
        return new PagedResult<ProductInfo>(productInfos, totalCount, pageIndex, pageSize);
    }

    /// <inheritdoc />
    public async Task<ProductInfo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : ToInfo(product);
    }

    /// <inheritdoc />
    public async Task<ProductInfo> CreateAsync(ProductInfo dto, CancellationToken cancellationToken = default)
    {
        var price = new Money(dto.Price);

        var product = new Product(dto.Name, dto.Description, dto.Sku, price, dto.StockQuantity);
        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("产品 {ProductId} ({ProductName}) 创建成功", product.Id, product.Name);
        return ToInfo(product);
    }

    /// <inheritdoc />
    public async Task<ProductInfo?> UpdateAsync(Guid id, ProductInfo dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("产品更新失败: 产品 {ProductId} 不存在", id);
            return null;
        }

        var price = new Money(dto.Price);
        product.UpdateInfo(dto.Name, dto.Description, price);

        // SKU 不可通过 UpdateInfo 修改，如需修改应创建新产品
        _logger.LogWarning("产品 {ProductId} SKU 未通过 UpdateInfo 修改（SKU: {Sku}）", product.Id, product.Sku);

        await _productRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("产品 {ProductId} ({ProductName}) 更新成功", product.Id, product.Name);
        return ToInfo(product);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("产品删除失败: 产品 {ProductId} 不存在", id);
            return false;
        }

        _productRepository.Remove(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("产品 {ProductId} ({ProductName}) 删除成功", product.Id, product.Name);
        return true;
    }

    /// <summary>
    /// 将产品实体转换为 DTO。
    /// </summary>
    /// <param name="product">产品实体。</param>
    /// <returns>产品信息 DTO。</returns>
    private static ProductInfo ToInfo(Product product)
    {
        return new ProductInfo
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Sku = product.Sku,
            Price = product.Price.Amount,
            StockQuantity = product.StockQuantity
        };
    }
}
