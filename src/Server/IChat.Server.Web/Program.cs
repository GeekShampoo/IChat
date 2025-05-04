using IChat.Server.Infrastructure.Data;
using IChat.Server.Infrastructure.Extensions;
using IChat.Server.Web.Extensions;
using IChat.Server.Web.Hubs;
using IChat.Server.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 添加数据库上下文和仓储服务
builder.Services.AddIChatDbContext(builder.Configuration);
builder.Services.AddRepositories();

// 添加核心业务服务
builder.Services.AddCoreServices();

// 添加认证服务
builder.Services.AddAuthServices(builder.Configuration);

// 添加控制器和API相关服务
builder.Services.AddControllers();
// 注册OpenAPI/Swagger服务
IChat.Server.Web.Extensions.OpenApiExtensions.AddOpenApi(builder.Services);

// 添加SignalR服务
builder.Services.AddSignalR();

// 注册ConnectionManager服务（用于管理用户连接）
builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();

var app = builder.Build();

// 初始化数据库
if (builder.Environment.IsDevelopment())
{
    // 在开发环境中初始化数据库（同步方法）
    DatabaseInitializer.Initialize(app.Services);
}
else
{
    // 在生产环境中异步初始化数据库
    // 注意：在实际生产环境中，可能需要更复杂的数据库迁移策略
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        Task.Run(async () =>
        {
            await DatabaseInitializer.InitializeAsync(app.Services);
        });
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

// 添加静态文件支持
app.UseStaticFiles();

// 添加默认文件支持
app.UseDefaultFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 配置SignalR终结点
app.MapHub<ChatHub>("/chathub");

app.Run();
