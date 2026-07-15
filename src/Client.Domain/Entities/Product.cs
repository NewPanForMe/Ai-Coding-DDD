using Client.Domain.Abstractions;
using Client.Domain.ValueObjects;

namespace Client.Domain.Entities;

/// <summary>
/// 产品聚合根，代表系统中的可销售产品。
/// </summary>
public sealed class Product : IAggregateRoot
{
    /// <summary>
    /// 初始化产品。
    /// </summary>
    /// <param name="name">产品名称。</param>
    /// <param name="description">产品描述。</param>
    /// <param name="sku">产品 SKU 编码。</param>
    /// <param name="price">产品单价。</param>
    /// <param name="stockQuantity">库存数量。</param>
    /// <exception cref="ArgumentException">名称为空、SKU 为空或价格无效时抛出。</exception>
    public Product(string name, string description, string sku, Money price, int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("产品名称不能为空。", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException("SKU 编码不能为空。", nameof(sku));
        }

        if (stockQuantity < 0)
        {
            throw new ArgumentException("库存数量不能为负数。", nameof(stockQuantity));
        }

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Sku = sku;
        Price = price;
        StockQuantity = stockQuantity;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 私有构造函数，供 EF Core 使用。
    /// </summary>
    private Product()
    {
        Name = string.Empty;
        Description = string.Empty;
        Sku = string.Empty;
        Price = new Money(0);
    }

    /// <summary>
    /// 获取产品唯一标识。
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// 获取产品名称。
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 获取产品描述。
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// 获取产品 SKU 编码。
    /// </summary>
    public string Sku { get; private set; }

    /// <summary>
    /// 获取产品单价。
    /// </summary>
    public Money Price { get; private set; }

    /// <summary>
    /// 获取库存数量。
    /// </summary>
    public int StockQuantity { get; private set; }

    /// <summary>
    /// 获取产品创建时间。
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 扣减库存。
    /// </summary>
    /// <param name="quantity">扣减数量。</param>
    /// <exception cref="InvalidOperationException">库存不足时抛出。</exception>
    public void DeductStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("扣减数量必须大于零。", nameof(quantity));
        }

        if (StockQuantity < quantity)
        {
            throw new InvalidOperationException(
                $"产品 {Name} 库存不足。当前库存: {StockQuantity}，需要: {quantity}。");
        }

        StockQuantity -= quantity;
    }

    /// <summary>
    /// 增加库存。
    /// </summary>
    /// <param name="quantity">增加数量。</param>
    public void AddStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("增加数量必须大于零。", nameof(quantity));
        }

        StockQuantity += quantity;
    }

    /// <summary>
    /// 更新产品基本信息。
    /// </summary>
    /// <param name="name">产品名称。</param>
    /// <param name="description">产品描述。</param>
    /// <param name="price">产品单价。</param>
    public void UpdateInfo(string name, string description, Money price)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }

        Description = description;
        Price = price;
    }
}
