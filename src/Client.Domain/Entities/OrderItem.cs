using Client.Domain.Abstractions;
using Client.Domain.ValueObjects;

namespace Client.Domain.Entities;

/// <summary>
/// 订单项实体，表示订单中的单个产品项。
/// </summary>
public sealed class OrderItem
{
    /// <summary>
    /// 初始化订单项。
    /// </summary>
    /// <param name="productId">产品 ID。</param>
    /// <param name="productName">产品名称。</param>
    /// <param name="sku">产品 SKU。</param>
    /// <param name="unitPrice">下单时单价。</param>
    /// <param name="quantity">购买数量。</param>
    /// <exception cref="ArgumentException">数量或单价无效时抛出。</exception>
    public OrderItem(Guid productId, string productName, string sku, Money unitPrice, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("购买数量必须大于零。", nameof(quantity));
        }

        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        Sku = sku;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    /// <summary>
    /// 私有构造函数，供 EF Core 使用。
    /// </summary>
    private OrderItem()
    {
        ProductName = string.Empty;
        Sku = string.Empty;
        UnitPrice = new Money(0);
    }

    /// <summary>
    /// 获取订单项唯一标识。
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// 获取产品 ID。
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// 获取产品名称（快照）。
    /// </summary>
    public string ProductName { get; private set; }

    /// <summary>
    /// 获取产品 SKU（快照）。
    /// </summary>
    public string Sku { get; private set; }

    /// <summary>
    /// 获取下单时单价。
    /// </summary>
    public Money UnitPrice { get; private set; }

    /// <summary>
    /// 获取购买数量。
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// 计算订单项总价。
    /// </summary>
    /// <returns>单价 × 数量。</returns>
    public Money GetTotalPrice() => UnitPrice.Multiply(Quantity);
}
