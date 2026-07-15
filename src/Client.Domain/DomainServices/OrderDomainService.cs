using Client.Domain.Entities;
using Client.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Client.Domain.DomainServices;

/// <summary>
/// 订单领域服务，封装跨聚合根的订单创建逻辑。
/// </summary>
public sealed class OrderDomainService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<OrderDomainService> _logger;

    /// <summary>
    /// 初始化订单领域服务。
    /// </summary>
    /// <param name="customerRepository">客户仓储。</param>
    /// <param name="productRepository">产品仓储。</param>
    /// <param name="logger">日志记录器。</param>
    public OrderDomainService(
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        ILogger<OrderDomainService> logger)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// 创建新订单，校验客户和产品库存。
    /// </summary>
    /// <param name="customerId">客户 ID。</param>
    /// <param name="items">订单项列表，每项包含产品 ID 和数量。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>创建的订单实体。</returns>
    /// <exception cref="InvalidOperationException">
    /// 当客户不存在、任一产品不存在或库存不足时抛出。
    /// </exception>
    public async Task<Order> CreateOrderAsync(
        Guid customerId,
        IReadOnlyList<(Guid productId, int quantity)> items,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer is null)
        {
            _logger.LogWarning("订单创建失败: 客户 {CustomerId} 不存在", customerId);
            throw new InvalidOperationException($"客户 {customerId} 不存在。");
        }

        var order = new Order(customer.Id, customer.Name, customer.Address);

        foreach (var (productId, quantity) in items)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null)
            {
                _logger.LogWarning("订单创建失败: 产品 {ProductId} 不存在", productId);
                throw new InvalidOperationException($"产品 {productId} 不存在。");
            }

            if (product.StockQuantity < quantity)
            {
                _logger.LogWarning(
                    "订单创建失败: 产品 {ProductId}({ProductName}) 库存不足，需要 {Required}，可用 {Available}",
                    productId, product.Name, quantity, product.StockQuantity);
                throw new InvalidOperationException(
                    $"产品 {product.Name}({productId}) 库存不足。需要 {quantity}，可用 {product.StockQuantity}。");
            }

            product.DeductStock(quantity);
            _productRepository.Update(product);

            var orderItem = new OrderItem(product.Id, product.Name, product.Sku, product.Price, quantity);
            order.AddOrderItem(orderItem);
        }

        order.Confirm();
        _logger.LogInformation("订单 {OrderId} 创建成功，客户: {CustomerName}", order.Id, customer.Name);

        return order;
    }
}
