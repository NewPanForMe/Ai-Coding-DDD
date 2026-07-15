using Client.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Client.Infrastructure.Data;

/// <summary>
/// 应用程序的 EF Core 数据库上下文，管理所有聚合根的持久化。
/// </summary>
public sealed class AppDbContext : DbContext
{
    /// <summary>
    /// 初始化数据库上下文。
    /// </summary>
    /// <param name="options">DbContext 配置选项。</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// 获取产品数据集。
    /// </summary>
    public DbSet<Product> Products { get; set; } = null!;

    /// <summary>
    /// 获取订单数据集。
    /// </summary>
    public DbSet<Order> Orders { get; set; } = null!;

    /// <summary>
    /// 获取客户数据集。
    /// </summary>
    public DbSet<Customer> Customers { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureProduct(modelBuilder);
        ConfigureCustomer(modelBuilder);
        ConfigureOrder(modelBuilder);
    }

    /// <summary>
    /// 配置产品实体映射。
    /// </summary>
    private static void ConfigureProduct(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Product>();

        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Sku).IsUnique();

        entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Description).HasMaxLength(1000);
        entity.Property(e => e.Sku).IsRequired().HasMaxLength(50);

        entity.ComplexProperty(e => e.Price, cfg =>
        {
            cfg.Property(m => m.Amount).IsRequired();
            cfg.Property(m => m.Currency).IsRequired().HasMaxLength(10);
        });

    }

    /// <summary>
    /// 配置客户实体映射。
    /// </summary>
    private static void ConfigureCustomer(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Customer>();

        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Email).IsUnique();

        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Phone).HasMaxLength(20);

        entity.ComplexProperty(e => e.Address, cfg =>
        {
            cfg.Property(a => a.Country).HasMaxLength(50);
            cfg.Property(a => a.Province).HasMaxLength(50);
            cfg.Property(a => a.City).HasMaxLength(50);
            cfg.Property(a => a.District).HasMaxLength(50);
            cfg.Property(a => a.Street).HasMaxLength(200);
            cfg.Property(a => a.PostalCode).HasMaxLength(20);
        });

        entity.Ignore(e => e.DomainEvents);
    }

    /// <summary>
    /// 配置订单实体映射。
    /// </summary>
    private static void ConfigureOrder(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Order>();

        entity.HasKey(e => e.Id);
        entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Status)
              .HasConversion<string>()
              .IsRequired()
              .HasMaxLength(20);

        entity.ComplexProperty(e => e.ShippingAddress, cfg =>
        {
            cfg.Property(a => a.Country).HasMaxLength(50);
            cfg.Property(a => a.Province).HasMaxLength(50);
            cfg.Property(a => a.City).HasMaxLength(50);
            cfg.Property(a => a.District).HasMaxLength(50);
            cfg.Property(a => a.Street).HasMaxLength(200);
            cfg.Property(a => a.PostalCode).HasMaxLength(20);
        });

        entity.OwnsMany(e => e.OrderItems, cfg =>
        {
            cfg.WithOwner().HasForeignKey("OrderId");
            cfg.HasKey(i => i.Id);
            cfg.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
            cfg.Property(i => i.Sku).HasMaxLength(50);
            cfg.Property(i => i.Quantity).IsRequired();

            cfg.OwnsOne(i => i.UnitPrice, moneyCfg =>
            {
                moneyCfg.Property(m => m.Amount).IsRequired();
                moneyCfg.Property(m => m.Currency).IsRequired().HasMaxLength(10);
            });
        });

        entity.Ignore(e => e.DomainEvents);
    }
}
