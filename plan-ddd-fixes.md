# Plan: DDD 代码规范修复 + 分层项目拆分（修订版 v2）

**创建时间**: 2026-07-15
**修订 v1**: 2026-07-15 — 用户要求保持 Program.cs 干净整洁
**修订 v2**: 2026-07-15 — 用户要求 DDD 每层独立为项目，Client 通过引用使用
**范围**: 26 个文件（9 个新增 + 6 个新建 + 11 个修改/移动）
**风险等级**: **高**（项目结构重构 + 代码修复并行）
**回滚策略**: 当前代码已全部读取完毕，可通过 git reset --hard 回滚

---

## 一、目标架构

```
DDD-Vue-AI-Rules/
├── DDD-Vue-AI-Rules.slnx            # 解决方案（更新）
├── src/
│   ├── Client.Domain/               # 领域层 — Class Library
│   │   ├── Client.Domain.csproj     # 无外部依赖（仅 Microsoft.Extensions.Logging）
│   │   ├── Abstractions/
│   │   │   ├── IAggregateRoot.cs
│   │   │   ├── IDomainEvent.cs
│   │   │   ├── IRepository.cs
│   │   │   └── ValueObject.cs
│   │   ├── Entities/
│   │   │   ├── Customer.cs
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   └── Product.cs
│   │   ├── ValueObjects/
│   │   │   ├── Address.cs
│   │   │   └── Money.cs
│   │   ├── DomainEvents/
│   │   │   ├── OrderCancelledDomainEvent.cs
│   │   │   └── OrderCreatedDomainEvent.cs
│   │   ├── DomainServices/
│   │   │   └── OrderDomainService.cs
│   │   └── Interfaces/
│   │       └── Repositories/
│   │           ├── ICustomerRepository.cs
│   │           ├── IOrderRepository.cs
│   │           └── IProductRepository.cs
│   │
│   ├── Client.Application/          # 应用层 — Class Library
│   │   ├── Client.Application.csproj  # →引用 Client.Domain
│   │   ├── Interfaces/
│   │   │   ├── ICustomerAppService.cs
│   │   │   ├── IOrderAppService.cs
│   │   │   └── IProductAppService.cs
│   │   ├── Services/
│   │   │   ├── CustomerAppService.cs
│   │   │   ├── OrderAppService.cs
│   │   │   └── ProductAppService.cs
│   │   ├── DTOs/
│   │   │   ├── CustomerInfo.cs
│   │   │   ├── OrderInfo.cs
│   │   │   ├── OrderItemInfo.cs
│   │   │   ├── ProductInfo.cs
│   │   │   └── PagedResult.cs
│   │   └── EventHandlers/           # 新增：领域事件处理器
│   │       └── OrderCancelledDomainEventHandler.cs
│   │
│   ├── Client.Infrastructure/       # 基础设施层 — Class Library
│   │   ├── Client.Infrastructure.csproj  # →引用 Client.Domain + Client.Application
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs
│   │   │   └── DbSeeder.cs          # 新增：种子数据
│   │   ├── Repositories/
│   │   │   ├── BaseRepository.cs
│   │   │   ├── CustomerRepository.cs
│   │   │   ├── OrderRepository.cs
│   │   │   └── ProductRepository.cs
│   │   ├── Middleware/              # 新增：全局异常处理
│   │   │   └── GlobalExceptionMiddleware.cs
│   │   └── DependencyInjection/
│   │       ├── InfrastructureServiceRegistration.cs
│   │       └── WebApplicationExtensions.cs   # 新增：扩展方法（Program.cs 用）
│   │
│   └── Client/                      # Web API 入口 — ASP.NET Core Web App
│       ├── Client.csproj            # →引用 Domain/Application/Infrastructure
│       ├── Program.cs               # ~12 行极简入口
│       ├── Controllers/
│       │   ├── CustomersController.cs
│       │   ├── OrdersController.cs
│       │   └── ProductsController.cs
│       ├── appsettings.json
│       └── appsettings.Development.json
│
└── tests/
    └── Client.Tests/                # 单元测试项目 — xUnit
        ├── Client.Tests.csproj      # →引用所有项目
        ├── Domain/
        │   ├── Entities/
        │   │   ├── CustomerTests.cs
        │   │   ├── OrderTests.cs
        │   │   └── ProductTests.cs
        │   └── Services/
        │       └── OrderDomainServiceTests.cs
        └── GlobalUsings.cs
```

---

## 二、项目依赖关系

```
Client.Tests
    ├── Client.Domain       (测试领域层)
    ├── Client.Application  (Mock 应用服务)
    └── Client.Infrastructure (暂时不需要，纯领域测试)

Client (Web API)
    ├── Client.Domain       (实体、值对象、仓储接口)
    ├── Client.Application  (应用服务接口、DTO)
    └── Client.Infrastructure (仓储实现、EF Core、中间件)

Client.Infrastructure
    ├── Client.Domain       (实现 IRepository)
    └── Client.Application  (事件处理器接口)

Client.Application
    └── Client.Domain       (使用实体和仓储接口)

Client.Domain
    └── (仅 Microsoft.Extensions.Logging.Abstractions)
```

---

## 三、执行步骤

### Phase 0: 项目结构搭建（新建 5 个项目）

| 步骤 | 操作 | 目标 | 说明 |
|------|------|------|------|
| **P0.1** | 新建 | `src/Client.Domain/Client.Domain.csproj` | `net9.0` Class Library，引用 `Microsoft.Extensions.Logging.Abstractions` |
| **P0.2** | 新建 | `src/Client.Application/Client.Application.csproj` | `net9.0` Class Library，引用 `Client.Domain` |
| **P0.3** | 新建 | `src/Client.Infrastructure/Client.Infrastructure.csproj` | `net9.0` Class Library，引用 `Client.Domain` + `Client.Application` + `Microsoft.EntityFrameworkCore.InMemory` |
| **P0.4** | 新建 | `src/Client/Client.csproj`（重写） | ASP.NET Core Web App，引用三个项目 + `Microsoft.AspNetCore.OpenApi` |
| **P0.5** | 新建 | `tests/Client.Tests/Client.Tests.csproj` | xUnit + Moq，引用 `Client.Domain` |

### Phase 1: 代码迁移（移动现有文件到新项目）

| 步骤 | 操作 | 源路径 | 目标路径 |
|------|------|--------|---------|
| **P1.1** | 移动 | `Client/Domain/**/*.cs` (14 个文件) | `src/Client.Domain/` |
| **P1.2** | 移动 | `Client/Application/**/*.cs` (11 个文件) | `src/Client.Application/` |
| **P1.3** | 移动 | `Client/Infrastructure/**/*.cs` (7 个文件) | `src/Client.Infrastructure/` |
| **P1.4** | 移动 | `Client/Controllers/*.cs` + `Program.cs` + `appsettings*.json` | `src/Client/` |
| **P1.5** | 删除 | 旧 `Client/` 目录 | 清理空壳 |
| **P1.6** | 更新 | `DDD-Vue-AI-Rules.slnx` | 注册 5 个项目 |

### Phase 2: 代码修复（在迁移后的新项目上执行）

| 分组 | 步骤 | 目标 | 修复内容 |
|------|------|------|---------|
| **日志** | P2.1 | `Client.Domain/DomainServices/OrderDomainService.cs` | 构造注入 `ILogger<OrderDomainService>`，记录 Warnings |
| | P2.2 | `Client.Application/Services/*.cs` (3 文件) | 构造注入 `ILogger<T>`，记录 CRUD |
| | P2.3 | `src/Client/Controllers/*.cs` (3 文件) | 构造注入 `ILogger<T>`，记录请求 |
| **异常处理** | P2.4 | `Client.Infrastructure/Middleware/GlobalExceptionMiddleware.cs` | 新建：全局异常 → ProblemDetails |
| | P2.5 | `Client.Infrastructure/DependencyInjection/WebApplicationExtensions.cs` | 新建：`UseApplicationMiddleware()` + `SeedDataAsync()` |
| **CancellationToken** | P2.6 | `src/Client/Controllers/*.cs` (3 文件) | All Actions + `CancellationToken` |
| **种子数据** | P2.7 | `Client.Infrastructure/Data/DbSeeder.cs` | 新建：提取种子数据 |
| **领域事件** | P2.8 | `Client.Infrastructure/Data/AppDbContext.cs` | 重写 SaveChangesAsync，分发 DomainEvents |
| | P2.9 | `Client.Application/EventHandlers/OrderCancelledDomainEventHandler.cs` | 新建：处理取消事件 |
| **Program.cs** | P2.10 | `src/Client/Program.cs` | 极简化：`AddApplicationServices()` → `SeedDataAsync()` → `UseApplicationMiddleware()` |
| **测试** | P2.11 | `tests/Client.Tests/Domain/**/*.cs` (4 文件) | 领域实体 + 领域服务测试 |
| | P2.12 | `tests/Client.Tests/Domain/Entities/OrderTests.cs` | 订单状态机测试 |

### Phase 3: 验证

| 步骤 | 操作 | 说明 |
|------|------|------|
| P3.1 | 编译 | `dotnet build` 全部通过 |
| P3.2 | 测试 | `dotnet test` 全部通过 |

---

## 四、Program.cs 最终形态（~12 行）

```csharp
using Client.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();          // 扩展方法：Controllers/OpenApi/EF Core/CORS

var app = builder.Build();

await app.SeedDataAsync();                  // 扩展方法：种子数据
app.UseApplicationMiddleware();             // 扩展方法：异常/OpenApi/CORS/HTTPS/Auth

app.Run();
```

---

## 五、变更汇总

| 类型 | 数量 | 说明 |
|------|------|------|
| **新建** | 5 | Domain/Application/Infrastructure/Client/Test 项目 .csproj |
| **移动** | 32 | 现有代码按层迁移到对应项目 |
| **修改** | 11 | 添加日志、CancellationToken、异常处理、事件分发 |
| **新增文件** | 4 | GlobalExceptionMiddleware, WebApplicationExtensions, DbSeeder, OrderCancelledDomainEventHandler |
| **新增测试** | 4 | CustomerTests, OrderTests, ProductTests, OrderDomainServiceTests |
| **删除** | 1 | 旧 Client/ 目录（迁移后空壳） |

---

## 六、风险评估

| 风险 | 等级 | 缓解措施 |
|------|------|---------|
| 命名空间变化 | 低 | 现有代码已使用 `Client.Domain.*` 等命名空间，不需修改 |
| 引用断裂 | 中 | 先建项目 → 移动代码 → 添加引用 → 编译验证 |
| Program.cs 破坏 | 低 | 扩展方法封装，改动隔离 |
| 旧文件残留 | 低 | Phase 1.5 显式清理 |

---

## 七、修复前后对比

| 维度 | 修复前 | 修复后 |
|------|--------|--------|
| 项目结构 | 1 个项目，所有代码混在一起 | **5 个项目**，严格分层 |
| 依赖方向 | 不明确 | Domain ← Application ← Infrastructure ← Client |
| 日志 | 0 处 | 7 个类 |
| 异常处理 | 500 裸奔 | 400/500 + ProblemDetails |
| CancellationToken | 控制器缺 | 12 个 Action 全有 |
| 测试 | 无 | 4 个测试类 |
| Program.cs | ~90 行 | **~12 行** |
