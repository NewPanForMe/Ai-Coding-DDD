namespace Client.Application.DTOs;

/// <summary>
/// 订单信息 DTO。
/// </summary>
public sealed class OrderInfo
{
    /// <summary>
    /// 获取或设置订单 ID。
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 获取或设置客户 ID。
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// 获取或设置客户姓名。
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置订单总金额。
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 获取或设置订单状态字符串。
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置创建时间。
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 获取或设置收货地址。
    /// </summary>
    public AddressInfo? Address { get; set; }

    /// <summary>
    /// 获取或设置订单项列表。
    /// </summary>
    public List<OrderItemInfo> Items { get; set; } = [];
}

/// <summary>
/// 地址信息 DTO。
/// </summary>
public sealed class AddressInfo
{
    /// <summary>
    /// 获取或设置国家。
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置省份。
    /// </summary>
    public string Province { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置城市。
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置区/县。
    /// </summary>
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置详细地址。
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置邮政编码。
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;
}
