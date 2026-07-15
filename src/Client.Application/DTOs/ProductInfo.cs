namespace Client.Application.DTOs;

/// <summary>
/// 产品信息 DTO。
/// </summary>
public sealed class ProductInfo
{
    /// <summary>
    /// 获取或设置产品 ID。
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 获取或设置产品名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置产品描述。
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置产品 SKU。
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置产品单价。
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 获取或设置货币代码。
    /// </summary>
    public string Currency { get; set; } = "CNY";

    /// <summary>
    /// 获取或设置库存数量。
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// 获取或设置创建时间。
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
