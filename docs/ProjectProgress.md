# IChat 项目进度记录

*最后更新时间：2025年5月3日*

## 项目概述

IChat 是一个基于 C# 开发的跨平台即时通讯系统，采用 Avalonia UI（客户端）和 ASP.NET Core（服务端）框架，通过 SignalR 实现实时通信功能。

## 当前开发阶段

根据项目规划，IChat 项目当前处于 **第一阶段：基础框架搭建** 的中期阶段。项目已完成基础架构搭建、领域模型定义和通信协议设计，正在向功能实现阶段过渡。

## 已完成工作

### 1. 项目规划与设计
- [x] 完成详细的项目规划文档《IMSystem综合说明.md》
- [x] 制定清晰的架构设计和技术选型
- [x] 规划项目开发路线图和时间安排

### 2. 项目结构搭建
- [x] 创建基础项目结构（客户端、服务端、共享代码）
- [x] 设置基本项目依赖关系
- [x] 初始化各子项目

### 3. 领域模型开发
- [x] 定义核心实体类
  - [x] BaseEntity 作为所有实体的基类
  - [x] User（用户）
  - [x] Message（消息）
  - [x] Group（群组）
  - [x] GroupMember（群成员）
  - [x] Conversation（会话）
  - [x] Friendship（好友关系）
  - [x] FileAttachment（文件附件）
  - [x] MessageReadReceipt（消息已读回执）
  - [x] Notification（通知）
  - [x] UserDevice（用户设备）
  - [x] UserLoginLog（登录日志）
  - [x] UserSetting（用户设置）
  - [x] UserToken（用户令牌）
- [x] 定义关键枚举类型
  - [x] ConversationType（会话类型）
  - [x] DeviceStatus（设备状态）
  - [x] DeviceType（设备类型）
  - [x] FileStatus（文件状态）
  - [x] FriendshipStatus（好友关系状态）
  - [x] GroupMemberRole（群成员角色）
  - [x] MessageStatus（消息状态）
  - [x] MessageType（消息类型）
  - [x] NotificationType（通知类型）
  - [x] TokenType（令牌类型）
  - [x] UserRole（用户角色）
  - [x] UserStatus（用户状态）

### 4. 通信协议设计
- [x] 设计基础消息结构（BaseMessage）
- [x] 定义各类消息模型
  - [x] 聊天消息（ChatMessageReceivedMessage）
  - [x] 消息状态更新（MessageStatusUpdatedMessage）
  - [x] 消息撤回通知（MessageRecalledMessage）
  - [x] 消息已读通知（MessagesReadMessage）
  - [x] 新消息通知（NewMessageNotificationMessage）
  - [x] 输入状态通知（TypingNotificationMessage）
- [x] 定义数据传输对象（DTOs）
  - [x] 消息 DTO（MessageDto）
  - [x] 消息引用 DTO（MessageReferenceDto）
  - [x] 消息已读回执 DTO（MessageReadReceiptDto）
  - [x] 发送消息请求（SendMessageRequest）
  - [x] 获取历史消息请求（GetHistoryMessagesRequest）
  - [x] 消息已读通知（MessageReadNotification）
  - [x] 消息撤回请求（RecallMessageRequest）
- [x] 按功能模块组织消息和 DTO
  - [x] Chat（聊天相关）
  - [x] Friend（好友相关）
  - [x] Group（群组相关）
  - [x] Notification（通知相关）
  - [x] Presence（在线状态相关）
  - [x] System（系统消息相关）
  - [x] Auth（认证相关）
  - [x] User（用户相关）
  - [x] File（文件相关）
  - [x] Conversation（会话相关）

### 5. 客户端开发
- [x] 初始化 Avalonia UI 框架
- [x] 设置基本的窗口和视图模型绑定
- [ ] 实现登录界面
- [ ] 实现主聊天界面
- [ ] 实现用户列表和群组列表

### 6. 服务端开发
- [x] 初始化 ASP.NET Core 框架
- [x] 实现用户认证和授权
- [x] 建立 SignalR Hub 处理实时通信
- [ ] 开发基本 RESTful API

## 正在进行的工作

1. **通信协议实现**
   - 已完成消息模型和数据传输对象的定义
   - [x] 已实现 SignalR Hub 来处理实时消息通信

2. **用户认证系统开发**
   - [x] 已完成用户注册和登录功能
   - [x] 已实现 JWT Token 认证机制
   - [x] 已实现刷新令牌机制和令牌管理

3. **客户端 UI 开发**
   - 正在设计登录界面和主界面布局
   - 准备实现基于 MVVM 模式的界面逻辑

4. **服务端通信与 API 开发**
   - [x] 已创建 ChatHub 类并实现实时通信处理逻辑
   - [x] 已开发 AuthController，实现了注册、登录、刷新令牌和注销功能
   - 其他 RESTful API 尚未开始开发

5. **客户端界面组件实现**
   - 未发现登录界面和主聊天界面相关 XAML 和 ViewModel，需要补充 LoginView、ChatView 等组件

## 最新进展 (2025年5月3日更新)

### 实时通信功能已实现

我们已经成功实现了基于 SignalR 的实时通信功能，这是项目通信层的重要进展。具体完成的工作包括：

1. **ChatHub 类实现**
   - [x] 创建 `ChatHub` 类并继承自 SignalR 的 Hub 基类
   - [x] 实现用户连接和断开连接的处理 (OnConnectedAsync, OnDisconnectedAsync)
   - [x] 创建 `ConnectionManager` 类管理用户的连接状态
   - [x] 配置 JWT 身份验证与 SignalR 集成

2. **消息传递功能**
   - [x] 实现私聊消息发送与接收功能
   - [x] 实现群组消息发送与接收功能
   - [x] 实现消息状态通知 (已发送、已送达、已读)
   - [x] 实现消息撤回功能
   - [x] 实现"正在输入"状态通知

3. **群组管理功能**
   - [x] 实现加入群组功能
   - [x] 实现离开群组功能
   - [x] 创建 SignalR 群组管理基础架构

4. **状态管理功能**
   - [x] 实现用户在线状态更新
   - [x] 为好友在线状态监控设计了框架
   - [x] 为群组成员状态通知设计了框架

5. **服务配置**
   - [x] 在 Program.cs 中注册 SignalR 服务
   - [x] 将 ChatHub 终结点映射到 "/chathub" 路径
   - [x] 注册 ConnectionManager 作为单例服务

### 用户认证与授权功能已完成

我们已经完成了用户认证和授权系统的开发，这是项目的一个重要里程碑，为后续功能开发奠定了基础。具体完成的工作包括：

1. **认证服务实现**
   - [x] 创建 `AuthService` 类，实现 `IAuthService` 接口
   - [x] 实现用户注册 (RegisterAsync) 功能，包括用户名和邮箱唯一性检查
   - [x] 实现用户登录 (LoginAsync) 功能，支持用户名或邮箱登录
   - [x] 实现密码哈希和验证功能，使用 BCrypt 进行安全的密码存储
   - [x] 实现令牌生成和验证功能，使用 JWT 作为访问令牌

2. **令牌管理功能**
   - [x] 实现 JWT 访问令牌生成和验证
   - [x] 实现刷新令牌机制，允许客户端在访问令牌过期后获取新令牌
   - [x] 实现令牌撤销功能，支持用户注销和安全退出
   - [x] 实现用户登录日志记录，跟踪用户登录设备和位置

3. **认证控制器实现**
   - [x] 创建 `AuthController` 实现用户注册 (Register) 端点
   - [x] 实现用户登录 (Login) 端点，返回访问令牌和刷新令牌
   - [x] 实现令牌刷新 (RefreshToken) 端点
   - [x] 实现用户注销 (Logout) 端点
   - [x] 实现获取当前用户信息 (GetCurrentUser) 端点

4. **安全性增强**
   - [x] 添加异常处理和日志记录
   - [x] 实现防止 SQL 注入的保护措施
   - [x] 实现输入验证和模型绑定验证
   - [x] 实现防止暴力破解的措施

### 数据库访问层实现完成

我们已经完成了数据库访问层的实现，这是项目的重要里程碑，为后续功能开发奠定了基础。具体完成的工作包括：

1. **数据库上下文设计与实现**
   - [x] 创建 `IChatDbContext` 类，继承自 Entity Framework Core 的 DbContext
   - [x] 配置所有实体的关系和约束
   - [x] 实现自动审计字段更新 (CreatedAt, UpdatedAt)
   - [x] 配置实体之间复杂的一对多、多对多关系
   - [x] 实现全局软删除查询过滤器

2. **仓储模式实现**
   - [x] 创建通用仓储接口 `IRepository<T>` 和实现类 `Repository<T>`
   - [x] 实现 `IUserRepository` 等特定实体的专用仓储
   - [x] 实现工作单元模式 (UnitOfWork)，管理事务和协调多仓储操作
   - [x] 提供丰富的数据查询和操作方法 (GetById, Find, GetPaged 等)

3. **数据库初始化与迁移**
   - [x] 创建 `DatabaseInitializer` 类，处理数据库创建和迁移
   - [x] 实现种子数据填充功能，添加管理员用户和基础配置
   - [x] 创建设计时 DbContext 工厂 `IChatDbContextFactory`
   - [x] 完成初始数据库迁移文件创建

4. **依赖注入配置**
   - [x] 创建 `ServiceCollectionExtensions` 类，集中配置数据库和仓储服务
   - [x] 更新 Program.cs，集成数据库上下文和初始化机制
   - [x] 配置 appsettings.json 中的数据库连接字符串

### 技术选型更新

- **ORM框架**：Entity Framework Core 9.0
- **数据库**：Microsoft SQL Server (LocalDB 用于开发)
- **密码哈希**：BCrypt.Net-Next 
- **认证框架**：JWT (JSON Web Token)
- **实时通信**：SignalR

### 已解决的问题

1. 数据库架构设计已完成并通过代码优先的方式实现
2. 解决了实体关系配置中的复杂导航属性和外键约束
3. 解决了软删除功能的实现，支持实体的逻辑删除而非物理删除
4. 解决了实体审计字段的自动更新机制
5. **解决了刷新令牌在数据库中存储和管理的问题**
6. **解决了用户认证和授权的实现问题**
7. **解决了设备ID、IP地址和位置信息等非空字段的处理问题**
8. **解决了实时通信基础设施的搭建问题**
9. **解决了用户连接状态管理的问题**

### 下一步计划调整

1. 实现消息服务 (IMessageService) 接口和实现类
2. 实现用户服务 (IUserService) 接口和实现类
3. 实现群组服务 (IGroupService) 接口和实现类
4. 实现客户端登录界面和与身份验证服务的集成
5. 实现客户端 SignalR 连接服务

### 实现进度更新

1. **已完成部分**：
   - 领域模型设计（实体、枚举、值对象）：100%
   - 通信协议定义（消息、请求/响应、DTO）：100%
   - 项目结构搭建：100% 
   - 数据访问层：90%
   - 用户认证和授权系统：100%
   - **实时通信基础设施（新增）：95%**

2. **进行中部分**：
   - 服务端实现：45%（已添加数据库上下文、仓储、认证功能和实时通信功能）
   - 客户端实现：15%（有基本的 MVVM 结构和主窗口）
   - 业务服务：25%（完成了认证服务，开始准备消息和好友服务）

### 遇到的技术难点补充

1. **Entity Framework Core 关系映射**
   - 问题：复杂的实体关系（如用户与好友关系的双向关联）导致导航属性配置困难
   - 解决方案：使用 Fluent API 明确配置每个关系，指定外键和导航属性

2. **软删除实现**
   - 问题：需要确保所有查询都自动排除已删除的实体，而不需要在每个查询中手动添加过滤条件
   - 解决方案：实现全局查询过滤器 (Query Filter)，自动应用于继承自 BaseEntity 的所有实体

3. **数据库初始化和种子数据**
   - 问题：避免重复创建种子数据、确保必要字段不为空
   - 解决方案：实现条件检查逻辑，只在数据库为空时添加初始数据，并为所有必需字段提供默认值

4. **JWT 令牌与刷新令牌机制**
   - 问题：需要安全地实现访问令牌和刷新令牌机制，同时跟踪用户的多设备登录
   - 解决方案：实现了基于 JWT 的访问令牌，并使用数据库存储刷新令牌，支持令牌撤销和重新生成

5. **参数验证和安全处理**
   - 问题：登录和注册过程中需要处理各种输入参数，包括处理 NULL 值和特殊字符
   - 解决方案：为所有可能为 NULL 的字段提供默认值，防止数据库插入错误，同时实现输入验证逻辑

6. **SignalR连接与状态管理**
   - 问题：当用户多终端登录时，需要维护所有连接并正确处理断开时的状态
   - 解决方案：实现了 ConnectionManager 服务管理用户连接，支持一个用户多个连接（多设备同时在线）

7. **实时消息可靠传递**
   - 问题：需要确保消息状态正确更新，离线用户返回时能接收到消息
   - 解决方案：设计了消息状态更新机制，为离线消息处理提供了框架

## 项目当前状态详细分析

### 服务端开发状态
1. **基础架构搭建**
   - 已创建 ASP.NET Core 8.0 项目基础结构
   - 已实现数据库连接和配置
   - 已实现依赖注入配置

2. **RESTful API 开发**
   - 已实现 AuthController，提供注册、登录、刷新令牌和注销功能
   - 其他控制器尚未实现

3. **SignalR 实时通信**
   - [x] 已创建 ChatHub 类并实现主要功能
   - [x] 已实现用户连接管理
   - [x] 已实现基本消息发送和接收功能
   - [x] 已配置 SignalR 终结点

4. **数据层**
   - 已实现数据库上下文和存储库模式
   - 已配置实体与数据库的映射关系
   - 已实现基本的数据访问功能

### 客户端开发状态
1. **Avalonia UI 基础架构**
   - 已创建基本的 Avalonia UI 应用程序结构
   - 包含 Program.cs 入口点和 App.axaml.cs 应用程序类
   - 实现了 MVVM 模式的基本结构 (ViewLocator, ViewModelBase)

2. **用户界面**
   - 只有默认的 MainWindow 主窗口
   - 尚未实现登录界面、聊天界面、联系人列表等功能界面
   - 没有自定义控件或样式定义

3. **客户端服务**
   - 尚未实现与服务器的通信服务
   - 未实现 SignalR 客户端连接
   - 缺少消息管理、用户管理等核心服务

### 通信协议开发状态
1. **消息模型**
   - 已完成所有消息类型的定义 (在 IChat.Protocol.Messages 命名空间下)
   - 包括系统消息、在线状态消息、通知消息、群组消息、好友消息和聊天消息

2. **数据传输对象**
   - 已完成所有 DTO 的定义 (在 IChat.Protocol.Dtos 命名空间下)
   - 包括用户、消息、群组、好友、文件和会话相关的 DTO

3. **请求/响应契约**
   - 已定义基础请求和响应类 (BaseRequest, BaseResponse)
   - 已完成身份认证相关的请求和响应定义

4. **实时通信**
   - [x] 已实现 ChatHub 类处理实时消息的接收和发送
   - [x] 实现了用户连接状态管理

### 领域模型开发状态
1. **实体定义**
   - 已完成所有核心实体的定义，包括用户、消息、群组、好友关系等
   - 实体间关系完善，包括一对多、多对多等关系

2. **枚举类型**
   - 已定义所有必要的枚举类型，如用户状态、消息类型、好友关系状态等

## 代码库详细分析

### 架构设计与项目结构

IChat 项目采用了分层架构设计，代码组织清晰，每个模块职责明确：

1. **Shared Layer**：包含所有共享代码，独立于客户端和服务端
   - **IChat.Domain**：定义核心领域模型和业务逻辑
   - **IChat.Protocol**：定义客户端和服务端通信协议
   - **IChat.Common**：提供通用工具和辅助功能（目前尚无实现）

2. **Server Layer**：服务端实现
   - **IChat.Server.Web**：ASP.NET Core Web API 主项目
   - **IChat.Server.Core**：服务端核心业务逻辑
   - **IChat.Server.Infrastructure**：数据访问和外部服务
   - **IChat.Server.Common**：服务端专用工具类

3. **Client Layer**：客户端实现
   - **IChat.Client.Avalonia**：Avalonia UI 界面
   - **IChat.Client.Core**：客户端核心逻辑
   - **IChat.Client.Common**：客户端专用工具类

4. **Tests Layer**：测试项目
   - **IChat.UnitTests**：单元测试
   - **IChat.IntegrationTests**：集成测试
   - **IChat.PerformanceTests**：性能测试

### 实现进度

1. **完成部分**：
   - 领域模型设计（实体、枚举、值对象）：100%
   - 通信协议定义（消息、请求/响应、DTO）：100%
   - 项目结构搭建：100%
   - 数据访问层：90%
   - 用户认证和授权系统：100%
   - **实时通信基础设施（新增）：95%**

2. **进行中部分**：
   - 服务端实现：45%（已添加数据库上下文、仓储、认证功能和实时通信功能）
   - 客户端实现：15%（有基本的 MVVM 结构和主窗口）
   - 业务服务：25%（完成了认证服务，开始准备消息和好友服务）

3. **待实现部分**：
   - 客户端 UI 界面：5%（仅有主窗口框架）
   - 消息服务、用户服务、群组服务：0%
   - 文件存储服务：0%
   - 单元测试和集成测试：0%

### 技术债务

1. **需要注意的区域**：
   - 客户端 UI 界面未完成
   - 缺少单元测试和集成测试
   - 消息持久化的具体实现

2. **潜在风险**：
   - 实时通信性能和可靠性需要验证
   - 离线消息处理机制需要完善

### 下一步优先任务

1. 实现核心业务服务（消息服务、用户服务、群组服务）
2. 实现客户端登录界面和与身份验证服务的集成
3. 实现客户端 SignalR 连接服务
4. 添加单元测试和集成测试

## 待解决的问题

1. 需要实现消息持久化存储机制
2. 需要完善离线消息处理策略
3. 需要实现客户端 SignalR 连接和断线重连机制
4. 需要设计和实现消息同步机制

## 下一步计划

1. 实现消息服务、用户服务和群组服务
2. 完善 ChatHub 与这些服务的集成
3. 实现客户端登录界面和与认证服务的集成
4. 实现客户端 SignalR 连接服务

## 技术笔记

### 当前技术选型
- **客户端**：Avalonia UI 11.0（MVVM架构）
- **服务端**：ASP.NET Core 8.0
- **通信**：SignalR
- **数据库**：Microsoft SQL Server (LocalDB 用于开发)
- **身份认证**：JWT Token

### 重要的设计决策
1. **消息机制**：采用基于 SignalR 的实时消息传递，支持多种消息类型和状态跟踪
2. **领域模型**：使用丰富的实体设计覆盖所有核心功能
3. **分层架构**：客户端和服务端均采用分层架构，确保代码的可维护性和可扩展性
4. **连接管理**：支持用户多设备同时在线，统一管理所有连接

### 遇到的技术难点
1. Avalonia UI 框架学习曲线
   - 解决方案：参考官方文档和示例项目

2. Entity Framework Core 关系映射
   - 解决方案：使用 Fluent API 明确配置

3. SignalR 连接管理
   - 解决方案：实现自定义的 ConnectionManager 服务

## 资源与参考

- [Avalonia UI 官方文档](https://docs.avaloniaui.net/)
- [ASP.NET Core 官方文档](https://learn.microsoft.com/en-us/aspnet/core/)
- [SignalR 官方文档](https://learn.microsoft.com/en-us/aspnet/core/signalr/)

---

*注：此进度记录文档会随项目进展定期更新*