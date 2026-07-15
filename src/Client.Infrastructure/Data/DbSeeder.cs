using Client.Domain.Entities;
using Client.Domain.ValueObjects;
using Client.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Client.Infrastructure.DependencyInjection;

/// <summary>
/// 种子数据初始化器，负责在开发环境中初始化示例数据。
/// </summary>
public sealed class DbSeeder
{
    private readonly AppDbContext _context;
    private readonly ILogger<DbSeeder> _logger;

    /// <summary>
    /// 初始化种子数据服务。
    /// </summary>
    /// <param name="context">数据库上下文。</param>
    /// <param name="logger">日志记录器。</param>
    public DbSeeder(AppDbContext context, ILogger<DbSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 执行种子数据初始化。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Customers.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("种子数据已存在，跳过初始化。");
            return;
        }

        _logger.LogInformation("开始初始化种子数据...");

        // 示例客户
        var customer1 = new Customer(
            "张三", "zhangsan@example.com", "13800001111",
            new Address("中国", "北京市", "北京市", "朝阳区", "建国路100号", "100020"));

        var customer2 = new Customer(
            "李四", "lisi@example.com", "13900002222",
            new Address("中国", "上海市", "上海市", "浦东新区", "陆家嘴金融街88号", "200120"));

        _context.Customers.AddRange(customer1, customer2);

        // 示例产品
        var products = new[]
        {
            new Product("机械键盘 K500", "RGB 背光 87 键机械键盘，Cherry MX 红轴", "KB-K500-RD",
                new Money(399.00m), 150),
            new Product("无线鼠标 M200", "2.4G 无线办公鼠标，静音按键", "MS-M200-BK",
                new Money(89.00m), 300),
            new Product("27寸 4K 显示器", "27 英寸 IPS 面板，3840×2160 分辨率，HDR400", "MN-4K27-IPS",
                new Money(2999.00m), 50),
            new Product("Type-C 扩展坞", "7 合 1 USB-C 扩展坞，支持 HDMI 4K@60Hz", "DK-TC71-GY",
                new Money(159.00m), 200),
            new Product("笔记本支架", "铝合金折叠散热支架，6 档高度调节", "ST-NB-AL",
                new Money(79.00m), 500),
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("种子数据初始化完成: {CustomerCount} 个客户, {ProductCount} 个产品",
            2, products.Length);
    }
}
