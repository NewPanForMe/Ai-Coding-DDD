namespace Client.Application.DTOs;

/// <summary>
/// 订单项信息 DTO。
/// </summary>
public sealed class OrderItemInfo
{
    /// <summary>
    /// 获取或设置订单项 ID。
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 获取或设置产品 ID。
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// 获取或设置产品名称。
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置产品 SKU。
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置单价。
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 获取或设置货币代码。
    /// </summary>
    public string Currency { get; set; } = "CNY";

    /// <summary>
    /// 获取或设置购买数量。
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 获取或设置小计金额。
    /// </summary>
    public decimal SubTotal { get; set; }
}
