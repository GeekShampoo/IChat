# IChat 即时通讯系统项目规划与架构说明文档

## 第一部分：项目概述与系统架构

### 1.1 项目概述

IChat 是一个基于 C# 开发的跨平台即时通讯系统，采用 Avalonia UI 作为客户端界面框架，ASP.NET Core 作为服务端框架，通过 SignalR 实现实时通信功能。系统设计为模块化、可扩展的分层架构，以支持高效的开发和维护。

### 1.2 整体架构
- **客户端**：使用C# Avalonia UI开发跨平台桌面应用
- **服务器端**：基于ASP.NET Core WebApi构建RESTful API和SignalR实时通信hub
- **数据库**：使用SQL Server存储用户数据和消息记录
- **通信方式**：结合WebSocket和SignalR实现实时消息传递

### 1.3 分层架构
- **表示层**：客户端UI界面
- **业务逻辑层**：处理核心业务逻辑
- **数据访问层**：与数据库交互
- **通信层**：负责客户端与服务器间通信

## 第二部分：项目结构与依赖关系

### 2.1 项目结构

IChat 采用分层架构，包含客户端、服务端和共享代码三大部分，具体结构如下：

```
IChat.sln
├── Client/                       # 客户端相关项目
├── Server/                       # 服务端相关项目
├── Shared/                       # 共享代码项目
└── docs/                         # 项目文档
```

#### 2.1.1 客户端项目结构

```
Client/
├── IChat.Client.Avalonia/     # Avalonia UI 客户端项目
├── IChat.Client.Core/         # 客户端核心业务逻辑
└── IChat.Client.Common/       # 客户端通用组件和工具类
```

#### 2.1.2 服务端项目结构

```
Server/
├── IChat.Server.Web/          # Web API 和 SignalR 服务
├── IChat.Server.Core/         # 服务端核心业务逻辑
├── IChat.Server.Infrastructure/ # 基础设施层
└── IChat.Server.Common/       # 服务端通用组件和工具类
```

#### 2.1.3 共享项目结构

```
Shared/
├── IChat.Domain/              # 领域模型
├── IChat.Common/              # 通用工具类
└── IChat.Protocol/            # 通信协议定义
```

#### 2.1.4 测试项目结构

```
Tests/
├── IChat.UnitTests/           # 单元测试
├── IChat.IntegrationTests/    # 集成测试
└── IChat.PerformanceTests/    # 性能测试
```

### 2.2 项目详细结构

#### 2.2.1 服务器端项目详细结构
```
IChat.Server.Web
├── Controllers            # API控制器
├── Hubs                   # SignalR Hubs
├── Middleware             # 中间件
├── Extensions             # 扩展方法
├── Filters                # 过滤器
├── Services               # API服务实现
├── Models                 # 视图模型(DTO)
└── Program.cs             # 启动文件

IChat.Server.Core
├── Services               # 核心业务服务
│   ├── UserService        # 用户服务
│   ├── MessageService     # 消息服务
│   ├── GroupService       # 群组服务
│   └── FileService        # 文件服务
├── Interfaces             # 服务接口定义
└── DomainEvents          # 领域事件

IChat.Server.Infrastructure
├── Data                   # 数据上下文
│   ├── AppDbContext       # EF Core数据上下文
│   └── Configurations     # 实体配置
├── Repositories           # 数据仓储实现
├── ExternalServices       # 外部服务集成
│   ├── StorageService     # 存储服务
│   └── NotificationService # 通知服务
└── BackgroundJobs         # 后台任务
```

#### 2.2.2 客户端项目详细结构
```
IChat.Client.Avalonia
├── App.axaml             # 应用定义
├── Views                 # 视图
│   ├── MainWindow        # 主窗口
│   ├── LoginView         # 登录
│   ├── ChatView          # 聊天
│   └── SettingsView      # 设置
├── ViewModels            # 视图模型
├── Models                # 数据模型
├── Services              # 服务
│   ├── ChatService       # 聊天服务
│   ├── UserService       # 用户服务
│   └── FileService       # 文件服务
├── Controls              # 自定义控件
├── Styles                # 样式定义
└── Assets                # 资源文件（图片、字体等）
```

### 2.3 项目依赖关系

#### 2.3.1 客户端依赖关系

- **IChat.Client.Avalonia**
  - 依赖于 IChat.Client.Core
  - 依赖于 IChat.Client.Common
  
- **IChat.Client.Core**
  - 依赖于 IChat.Client.Common
  - 依赖于 IChat.Domain（共享）
  - 依赖于 IChat.Protocol（共享）

- **IChat.Client.Common**
  - 无内部依赖

#### 2.3.2 服务端依赖关系

- **IChat.Server.Web**
  - 依赖于 IChat.Server.Core
  - 依赖于 IChat.Server.Infrastructure
  - 依赖于 IChat.Server.Common
  - 依赖于所有共享项目（Domain、Protocol、Common）

- **IChat.Server.Core**
  - 定义接口（服务接口、仓储接口等）
  - 依赖于 IChat.Server.Common
  - 依赖于 IChat.Domain（共享）
  - 依赖于 IChat.Protocol（共享）
  - **不直接依赖** IChat.Server.Infrastructure

- **IChat.Server.Infrastructure**
  - 实现 Core 层定义的仓储接口
  - 依赖于 IChat.Server.Core（**仅引用接口，不调用具体服务**）
  - 依赖于 IChat.Server.Common
  - 依赖于 IChat.Domain（共享）

- **IChat.Server.Common**
  - 无内部依赖

#### 2.3.3 共享项目依赖

- **IChat.Domain**
  - 无内部依赖
  
- **IChat.Common**
  - 无内部依赖
  
- **IChat.Protocol**
  - 可能依赖于 IChat.Domain（实体模型）

## 第三部分：核心功能模块与技术选型

### 3.1 核心功能模块

#### 3.1.1 用户管理模块
- 注册和登录功能
- 用户个人资料维护
- 状态管理（在线、离线、忙碌等）
- 账号安全（密码重置、安全验证）

#### 3.1.2 好友关系模块
- 添加、删除好友
- 好友分组管理
- 好友状态查看
- 好友查找功能

#### 3.1.3 消息管理模块
- 私聊消息收发
- 群聊消息处理
- 消息历史记录
- 消息已读/未读状态
- 消息撤回功能

#### 3.1.4 群组管理模块
- 创建、解散群组
- 群成员管理（邀请、踢出）
- 群公告、群设置
- 群权限管理

#### 3.1.5 文件传输模块
- 图片、文档、视频等文件发送
- 文件进度显示
- 文件存储与管理

#### 3.1.6 音视频通话模块
- 一对一语音/视频通话
- 群组语音/视频会议
- 音视频质量控制

### 3.2 各项目详细说明

#### 3.2.1 客户端项目

##### IChat.Client.Avalonia

**主要职责**：
- 提供用户界面和交互
- 实现 MVVM 模式的视图和视图模型
- 处理用户输入和界面事件

**关键目录**：
- `Views/`: 包含所有 UI 界面
- `ViewModels/`: 包含视图模型
- `Services/`: 客户端特定服务实现
- `Controls/`: 自定义 UI 控件
- `Styles/`: UI 样式定义
- `Assets/`: 资源文件（图片、图标等）

**主要依赖包**：
- Avalonia UI 框架
- ReactiveUI (MVVM 支持)
- Avalonia.ReactiveUI (Avalonia 与 ReactiveUI 集成)

##### IChat.Client.Core

**主要职责**：
- 实现客户端业务逻辑
- 管理客户端状态
- 处理与服务器的通信

**关键目录**：
- `Services/`: 核心业务服务实现
- `Interfaces/`: 服务接口定义
- `Models/`: 客户端特定数据模型

**主要依赖包**：
- SignalR.Client (实时通信)

##### IChat.Client.Common

**主要职责**：
- 提供客户端通用工具类
- 提供辅助功能和扩展方法

#### 3.2.2 服务端项目

##### IChat.Server.Web

**主要职责**：
- 提供 RESTful API 接口
- 实现 SignalR Hub 处理实时通信
- 处理 HTTP 请求和响应

**关键目录**：
- `Controllers/`: API 控制器
- `Hubs/`: SignalR Hub 定义
- `Middleware/`: 自定义中间件
- `Extensions/`: 应用扩展方法
- `Filters/`: 请求过滤器
- `Models/`: 视图模型/DTO
- `Services/`: Web 层服务实现

**主要依赖包**：
- ASP.NET Core
- SignalR

##### IChat.Server.Core

**主要职责**：
- 实现核心业务逻辑
- 定义业务规则和服务接口
- 处理领域事件

**关键目录**：
- `Services/`: 业务服务实现
- `Interfaces/`: 接口定义
- `DomainEvents/`: 领域事件定义

**主要依赖包**：
- AutoMapper
- FluentValidation

##### IChat.Server.Infrastructure

**主要职责**：
- 实现数据访问层
- 管理外部资源和服务集成
- 处理后台任务

**关键目录**：
- `Data/`: 数据库上下文和配置
- `Repositories/`: 数据仓储实现
- `ExternalServices/`: 外部服务集成
- `BackgroundJobs/`: 后台任务

**主要依赖包**：
- Entity Framework Core
- SQL Server Provider
- Redis 缓存

##### IChat.Server.Common

**主要职责**：
- 提供服务端通用工具类
- 提供辅助功能和扩展方法

#### 3.2.3 共享项目

##### IChat.Domain

**主要职责**：
- 定义核心领域模型/实体
- 定义业务规则和约束

**关键目录**：
- `Entities/`: 领域实体定义

##### IChat.Common

**主要职责**：
- 提供所有项目共享的工具类
- 定义常量和枚举
- 提供通用的扩展方法

##### IChat.Protocol

**主要职责**：
- 定义客户端和服务端通信协议
- 定义消息格式和结构
- 定义 API 契约

### 3.3 技术选型详解

#### 3.3.1 客户端技术
- **UI框架**：Avalonia UI 11.0（MVVM架构）
  - 跨平台UI框架，支持Windows、macOS、Linux、Android、iOS和WebAssembly
  - 使用XAML进行UI设计，类似WPF但更现代化
  - 内置FluentUI设计语言支持
- **MVVM框架**：**ReactiveUI**
  - 强大的响应式 MVVM 框架，与 Avalonia 集成良好，适合处理异步和复杂 UI 交互。
- **界面库**：
  - Avalonia.Fluent - 提供Fluent设计体验
  - Suki UI - 现代化的UI控件库，提供美观且高度可定制的控件集合
- **本地存储**：SQLite保存本地聊天记录
- **网络通信**：HttpClient处理HTTP请求，SignalR.Client处理实时通信

#### 3.3.2 服务器端技术
- **Web框架**：ASP.NET Core 8.0
- **实时通信**：SignalR实现消息推送
- **API设计**：RESTful API处理非实时请求
- **依赖注入**：使用内置DI容器
- **身份认证**：JWT Token认证

#### 3.3.3 数据存储技术
- **ORM框架**：Entity Framework Core
- **数据库**：SQL Server
- **缓存**：Redis缓存热点数据
- **文件存储**：对象存储服务或本地文件系统

#### 3.3.4 通信协议
- **消息格式**：JSON
- **实时通信**：WebSocket + SignalR
- **文件传输**：分片上传，断点续传

### 3.4 安全设计

#### 3.4.1 数据安全
- 消息端到端加密
- 敏感数据加密存储
- 传输过程HTTPS加密

#### 3.4.2 用户认证与授权
- 多因素认证
- 基于角色的访问控制
- JWT令牌管理

## 第四部分：数据库设计与部署架构

### 4.1 数据库设计

#### 4.1.1 核心表结构
- **Users表**：用户信息
- **Friends表**：好友关系
- **Groups表**：群组信息
- **GroupMembers表**：群成员关系
- **Messages表**：消息记录
- **Files表**：文件信息

### 4.2 技术栈概述

#### 4.2.1 客户端技术栈
- **UI 框架**: Avalonia UI 
- **架构模式**: MVVM (Model-View-ViewModel)
- **通信**: SignalR.Client, HttpClient
- **本地存储**: SQLite (计划中)

#### 4.2.2 服务端技术栈
- **Web 框架**: ASP.NET Core 8.0
- **实时通信**: SignalR
- **ORM**: Entity Framework Core
- **数据库**: SQL Server
- **缓存**: Redis
- **认证**: JWT Token

### 4.3 部署架构

#### 4.3.1 开发环境
- **客户端**: Windows/macOS/Linux 桌面平台
- **服务端**: 本地开发服务器
- **数据库**: 本地 SQL Server 实例

#### 4.3.2 生产环境 (计划)
- **客户端**: 分发为各平台安装包
- **服务端**: 云托管服务或本地服务器
- **数据库**: 云数据库服务或本地服务器
- **缓存**: 云缓存服务或本地服务器

## 第五部分：开发计划与挑战解决方案

### 5.1 开发路线图

#### 5.1.1 第一阶段：基础框架搭建（1-2个月）
- 搭建客户端和服务器基础架构
- 实现用户注册和登录功能
- 建立基本的通信机制

#### 5.1.2 第二阶段：核心功能开发（2-3个月）
- 开发好友管理功能
- 实现一对一聊天功能
- 开发群组创建和管理功能
- 实现群聊功能

#### 5.1.3 第三阶段：高级功能开发（2-3个月）
- 开发文件传输功能
- 实现语音/视频通话
- 添加表情、动画等富媒体消息

#### 5.1.4 第四阶段：性能优化和安全加固（1-2个月）
- 性能测试和优化
- 安全审计和加固
- 多设备同步机制完善

#### 5.1.5 第五阶段：测试和部署（1个月）
- 全面测试
- 用户体验优化
- 部署上线

### 5.2 开发流程

1. **领域模型开发**: 首先完善共享项目中的领域模型
2. **服务端 API 开发**: 实现核心 API 和 SignalR Hub
3. **客户端核心逻辑开发**: 实现与服务端通信的核心逻辑
4. **UI 开发**: 构建 Avalonia 用户界面
5. **集成测试**: 测试客户端和服务端交互

### 5.3 关键技术挑战及解决方案

#### 5.3.1 实时消息传递
- **挑战**：确保消息实时可靠传递
- **解决方案**：使用SignalR持久连接，结合消息队列确保可靠性

#### 5.3.2 离线消息处理
- **挑战**：用户离线时的消息存储与同步
- **解决方案**：服务器存储离线消息，用户上线后自动推送

#### 5.3.3 大规模并发
- **挑战**：处理大量用户同时在线
- **解决方案**：服务器集群、负载均衡、消息分区

#### 5.3.4 安全性保障
- **挑战**：保证通信和数据安全
- **解决方案**：全程HTTPS、消息加密、定期安全审计

### 5.4 后续开发计划

1. 完善用户认证与授权系统
2. 实现实时消息传递功能
3. 开发群组和好友管理功能
4. 实现文件传输功能
5. 添加音视频通话功能
6. 性能优化和安全加固

## 第六部分：第三方库与工具建议

### 6.1 建议使用的第三方库

1. **SignalR** - 实时通信
2. **Entity Framework Core** - ORM框架
3. **AutoMapper** - 对象映射
4. **Serilog** - 日志记录
5. **FluentValidation** - 数据验证
7. **NAudio** - 音频处理
8. **ImageSharp** - 图像处理
9. **ReactiveUI** - 响应式 MVVM 框架 (客户端核心)
10. **Avalonia.ReactiveUI** - Avalonia 与 ReactiveUI 集成
11. **Avalonia.Controls.DataGrid** - 增强数据表格控件
12. **Avalonia.Xaml.Behaviors** - 行为支持库
13. **Avalonia.Skia** - 高性能渲染库
14. **Newtonsoft.Json/System.Text.Json** - JSON处理