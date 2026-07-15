using Client.Application.EventHandlers;
using Client.Application.Interfaces;
using Client.Application.Services;
using Client.Domain.DomainServices;
using Client.Domain.Interfaces.Repositories;
using Client.Infrastructure.Data;
using Client.Infrastructure.Middleware;
using Client.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client.Infrastructure.DependencyInjection;

/// <summary>
/// Web 应用程序扩展方法，封装服务注册、中间件管道和种子数据初始化。
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// 注册应用程序所有服务到 DI 容器。
    /// </summary>
    /// <param name="builder">Web 应用程序构建器。</param>
    /// <returns>Web 应用程序构建器（支持链式调用）。</returns>
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // EF Core InMemory 数据库
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("DddDemoDb"));

        // 种子数据服务
        builder.Services.AddScoped<DbSeeder>();

        // DDD 分层服务注册
        builder.Services.AddInfrastructureServices();

        // CORS 配置（允许 Vue 前端开发服务器访问）
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return builder;
    }

    /// <summary>
    /// 配置应用程序中间件管道。
    /// </summary>
    /// <param name="app">Web 应用程序。</param>
    /// <returns>Web 应用程序（支持链式调用）。</returns>
    public static WebApplication UseApplicationMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseCors();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    /// <summary>
    /// 初始化种子数据。
    /// </summary>
    /// <param name="app">Web 应用程序。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    public static async Task SeedDataAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
        await seeder.SeedAsync(cancellationToken);
    }

    /// <summary>
    /// 注册基础设施层所有服务到 DI 容器。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合（支持链式调用）。</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // 仓储注册
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // 领域服务注册
        services.AddScoped<OrderDomainService>();

        // 应用服务注册
        services.AddScoped<ICustomerAppService, CustomerAppService>();
        services.AddScoped<IProductAppService, ProductAppService>();
        services.AddScoped<IOrderAppService, OrderAppService>();

        // 领域事件处理器注册
        services.AddScoped<Application.EventHandlers.OrderCancelledDomainEventHandler>();

        return services;
    }
}
