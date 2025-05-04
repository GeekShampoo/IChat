using IChat.Server.Infrastructure.Data;
using IChat.Domain.Entities;
using IChat.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IChat.Server.Infrastructure.Data
{
    /// <summary>
    /// 数据库初始化器，负责数据库的创建、迁移和初始数据的填充
    /// </summary>
    public class DatabaseInitializer
    {
        /// <summary>
        /// 初始化数据库（同步方法）
        /// </summary>
        /// <param name="serviceProvider">服务提供程序</param>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<IChatDbContext>();
                Initialize(dbContext);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<DatabaseInitializer>>();
                logger.LogError(ex, "初始化数据库时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 初始化数据库（异步方法）
        /// </summary>
        /// <param name="serviceProvider">服务提供程序</param>
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<IChatDbContext>();
                await InitializeAsync(dbContext);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<DatabaseInitializer>>();
                logger.LogError(ex, "初始化数据库时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 初始化数据库（同步方法，直接使用DbContext）
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public static void Initialize(IChatDbContext dbContext)
        {
            // 确保数据库已创建并应用所有迁移
            dbContext.Database.Migrate();
            
            // 填充初始数据
            SeedData(dbContext);
        }

        /// <summary>
        /// 初始化数据库（异步方法，直接使用DbContext）
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public static async Task InitializeAsync(IChatDbContext dbContext)
        {
            // 确保数据库已创建并应用所有迁移
            await dbContext.Database.MigrateAsync();
            
            // 填充初始数据
            await SeedDataAsync(dbContext);
        }

        /// <summary>
        /// 填充初始数据（同步方法）
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        private static void SeedData(IChatDbContext dbContext)
        {
            // 仅当数据库中没有用户时才添加管理员用户
            if (!dbContext.Users.Any())
            {
                SeedAdminUser(dbContext);
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// 填充初始数据（异步方法）
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        private static async Task SeedDataAsync(IChatDbContext dbContext)
        {
            // 仅当数据库中没有用户时才添加管理员用户
            if (!await dbContext.Users.AnyAsync())
            {
                SeedAdminUser(dbContext);
                await dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 填充管理员用户
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        private static void SeedAdminUser(IChatDbContext dbContext)
        {
            // 创建管理员用户
            var adminUser = new IChat.Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                // 实际应用中应该使用更安全的密码哈希方法，这里使用临时明文密码，需要添加BCrypt包
                PasswordHash = "Admin@123", // 后续添加 BCrypt 包后改为：BCrypt.Net.BCrypt.HashPassword("Admin@123")
                PasswordSalt = Guid.NewGuid().ToString(), // 临时盐值
                Email = "admin@ichat.com",
                Nickname = "系统管理员",
                Role = UserRole.SystemAdmin, // 修正为实体中定义的 Administrator
                Status = UserStatus.Online,
                AvatarUrl = "/assets/default-avatar.png", // 添加默认头像URL
                PhoneNumber = "", // 添加空字符串而不是NULL
                Signature = "", // 添加空字符串而不是NULL
                CreatedAt = DateTime.UtcNow
            };

            // 添加管理员用户
            dbContext.Users.Add(adminUser);

            // 创建并添加管理员用户设置
            var adminSettings = new IChat.Domain.Entities.UserSetting
            {
                Id = Guid.NewGuid(),
                UserId = adminUser.Id,
                ThemeMode = 1, // 修正字段名，使用实体中定义的 ThemeMode
                ThemeColor = "#1890ff", // 添加主题颜色
                EnableNotification = true, // 修正字段名，使用实体中定义的 EnableNotification
                EnableSoundNotification = true,
                ShowMessagePreview = true,
                NotificationSound = "default",
                Language = "zh-CN",
                AutoLogin = true,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.UserSettings.Add(adminSettings);
        }
    }
}