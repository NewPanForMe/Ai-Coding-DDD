# Project Templates — Rule Set by Project Type

不同项目类型的完整规则配置模板。当为具体项目设置规则时，参考对应模板进行定制。

---

## 前端项目（React / Vue / Angular）

### 代码范围

- **strict**：组件级别的样式修改
- **file**：单组件逻辑修改
- **module**：页面级别（页面组件 + 子组件 + hooks/store）
- **禁止修改**：`package.json`（依赖锁）、路由配置、全局状态结构、认证逻辑

### 组件规范

```
src/components/
├── Button/
│   ├── Button.tsx          # 组件主体
│   ├── Button.module.css   # 样式隔离（CSS Modules）
│   ├── Button.test.tsx     # 单元测试
│   └── index.ts            # 导出入口
```

- 组件目录名 = 组件名（PascalCase）
- 样式使用 CSS Modules 或 styled-components，禁止内联 style
- 每个组件需要有 loading、error、empty 状态处理
- 容器组件与展示组件分离

### 状态管理

- 本地状态用 `useState` / `ref`
- 跨组件共享用 Context / provide-inject
- 全局状态用 store（Zustand / Pinia / Redux Toolkit）
- 服务端状态用 React Query / SWR / VueUse

### 禁止事项

- 禁止在渲染期间修改 state（引起无限循环）
- 禁止在 useEffect 中遗漏依赖项
- 禁止使用 `any` 类型（TypeScript 项目）
- 禁止直接操作 DOM（除非必要且注释说明）

---

## 后端项目（Node.js / Python / Go）

### 代码范围

- **strict**：单个 handler / controller 逻辑
- **file**：单文件内的多个 handler
- **module**：同一资源的所有层（controller + service + repository）
- **禁止修改**：数据库 schema / migration、中间件链、认证鉴权逻辑、环境变量配置

### 分层架构

```
src/
├── controllers/   # 请求处理，参数校验，响应格式化
├── services/      # 业务逻辑
├── repositories/  # 数据访问层
├── models/        # 数据模型/实体定义
├── middleware/    # 中间件
├── utils/         # 工具函数
├── config/        # 配置
└── routes/        # 路由定义
```

- 层间调用方向固定：Controller → Service → Repository（不可反向）
- Service 层不能直接操作 req/res 对象
- Repository 层只做数据访问，不含业务逻辑

### API 设计

- RESTful 命名：名词复数（`/users`，`/orders`）
- 统一响应格式：
  ```json
  { "code": 0, "data": {}, "message": "ok" }
  ```
- 错误码 0 为成功，非 0 按分类定义
- 所有 API 需要参数校验（不允许信任用户输入）

### 禁止事项

- 禁止在循环中执行数据库查询（N+1 问题）
- 禁止密码/密钥硬编码（使用环境变量或配置服务）
- 禁止同步阻塞操作在异步上下文中
- 禁止不设超时的外部调用

---

## .NET / C# 后端项目

> 基于 DotNetSpec v1.2 规范体系，运行时框架 `.NET Core 3.1+` 或 `.NET 6+`，C# 语言版本 7.0+。

### 代码范围

- **strict**：单个 Controller Action / Service 方法
- **file**：单个 Controller / Service / Repository 文件
- **module**：同一业务模块（Controller + Service + Repository + DTO + Entity）
- **禁止修改**：`*.csproj`（依赖和构建配置）、`appsettings.json`（环境配置）、已执行的 EF Core Migration、`Program.cs` / `Startup.cs`（启动配置）、认证鉴权中间件

### 分层架构与命名空间

```
Solution/
├── src/
│   ├── {Company}.{Project}.Presentation/    # 表现层
│   │   ├── Controllers/                      # WebApi Controllers
│   │   ├── ViewModels/                       # VO (View Objects)
│   │   └── Filters/                          # Action Filters
│   ├── {Company}.{Project}.Application/      # 应用层
│   │   ├── Services/                         # 应用服务
│   │   ├── DataTransferObjects/              # DTO
│   │   └── Specifications/                   # SO (查询规约)
│   ├── {Company}.{Project}.Domain/           # 领域层
│   │   ├── Entities/                         # DO (Domain Objects)
│   │   ├── DomainServices/                   # 领域服务
│   │   ├── DomainEvents/                     # 领域事件
│   │   └── Repositories/                     # 仓储接口
│   └── {Company}.{Project}.Infrastructure/   # 基础设施层
│       ├── Data/                             # EF Core DbContext, Migrations
│       ├── Repositories/                     # 仓储实现
│       └── ExternalServices/                 # 外部服务集成
└── test/
    └── {Company}.{Project}.Tests/            # 单元测试
```

- 命名空间最多 5 级（如 `Company.Project.Domain.Entities`）
- 依赖方向：Presentation → Application → Domain ← Infrastructure
- Domain 层不依赖任何外部层（核心原则）

### 对象命名规范

| 类型 | 后缀 | 示例 |
| --- | --- | --- |
| Domain Object（实体） | 无后缀，名词复数 | `Users`, `Orders` |
| Data Transfer Object | `Info` | `UserInfo`, `OrderInfo` |
| View Object | `ViewModel`，格式：`模块+操作+ViewModel` | `UserCreateViewModel`, `MembershipDisplayListViewModel` |
| Specification Object | `Spec` | `UserListSpec`, `OrderQuerySpec` |
| 仓储接口 | `IRepository` | `IUserRepository` |
| 服务接口 | `IService` | `IUserService` |

### C# 命名速查表

| 元素 | 规则 | 示例 |
| --- | --- | --- |
| 私有成员变量 | `_camelCase` | `_userId`, `_dbContext` |
| 局部变量/参数 | `camelCase` | `userName`, `orderId` |
| 常量 | `UPPER_SNAKE_CASE` | `MAX_RETRY_COUNT` |
| 接口 | `IPascalCase` | `IUserRepository` |
| 抽象类 | `PascalCase` + `Base`/`Provider` | `RepositoryBase` |
| 异常类 | `PascalCase` + `Exception` | `UserNotFoundException` |
| 布尔变量 | `Is`/`Has`/`Can` 前缀 | `isActive`, `hasPermission` |
| 异步方法 | `PascalCase` + `Async` | `GetUserAsync()` |
| 扩展方法类 | `PascalCase` + `Extensions` | `StringExtensions` |
| 测试类 | 被测试类名 + `Test` | `UserServiceTest` |
| 测试方法 | `Test_` + 描述（下划线分隔） | `Test_GetUser_WhenNotFound_ThrowsException` |

### 代码格式 .editorconfig 参考

```ini
[*.cs]
indent_style = space
indent_size = 4
max_line_length = 120
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true
```

### 异常与日志

- 使用 `ILogger<T>` 门面接口，不直接依赖具体日志实现
- 生产环境不输出 Debug 日志
- 异常日志包含案发现场信息（参数值）和完整堆栈
- warn 级别用于用户输入错误；error 级别仅用于系统逻辑错误
- 不捕获可通过常规检查避免的异常（如 NullReferenceException）
- 日志应该完整配置，包含详细信息与数据库等，日志默认存于数据库

### 单元测试

- 测试项目使用 xUnit / NUnit / MSTest
- 测试目录：`test/{Project}.Tests/`
- AIR 原则：全自动、独立、可重复
- BCDE 原则：边界值、正确输入、设计文档对齐、错误输入
- 覆盖目标：整体 70%，核心模块 100%
- 数据库测试使用 InMemory Database 或自动回滚机制
- 通过 DI + Moq/NSubstitute 隔离外部依赖

### 禁止事项

- 禁止字符串拼接（循环中用 StringBuilder）
- 禁止使用 Tuple，改用命名元组
- 禁止使用 `List<T>` 暴露集合属性（用 `IEnumerable<T>` 或 `IReadOnlyList<T>`）
- 禁止用 `Count() > 0` 判断集合非空（用 `Any()`）
- 禁止使用已过时的 API
- 禁止修改已发布接口的签名（用扩展方法兼容）
- 禁止在 finally 块中使用 return
- 禁止超过 3 层 if-else 嵌套

---

## 全栈项目（Next.js / Nuxt / Remix）

继承前端 + 后端所有规则，额外补充：

### 代码范围

- 前端和后端的修改范围独立计算
- 禁止一次修改同时跨前端和后端边界（除非关联修改必须在同一 PR）

### SSR 专项

- Server Component 不能使用浏览器 API（`window`、`document`、`localStorage`）
- 必须显式标记 `'use client'` 边界
- 服务端数据获取需要错误边界和加载状态

### 数据库操作

- 所有数据库操作走 ORM/Query Builder，禁止拼接 SQL 字符串
- 变更数据库结构必须有对应的 migration 文件
- Migration 禁止包含数据删除操作（DROP COLUMN 等需要特别标记）

---

## 移动端项目（React Native / Flutter）

### 额外规范

- 图片资源统一管理在 `assets/` 目录
- 适配多分辨率（至少 @2x, @3x）
- 网络请求需要有离线/弱网降级策略
- 平台差异代码用 `Platform.OS` 分离（React Native）或 `dart:io` 平台判断（Flutter）
