using IChat.Domain.Entities;
using IChat.Domain.Interfaces;
using IChat.Server.Infrastructure.Data;
using IChat.Server.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IChat.Server.Infrastructure.Extensions
{
    /// <summary>
    /// 数据库和仓储层的服务注册扩展
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// 添加数据库上下文和相关服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddIChatDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册数据库上下文
            services.AddDbContext<IChatDbContext>(options =>
            {
                // 从配置中获取连接字符串
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                
                // 使用SQL Server
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    // 启用自动迁移重试
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    
                    // 设置迁移程序集
                    sqlOptions.MigrationsAssembly("IChat.Server.Infrastructure");
                });
            });

            return services;
        }

        /// <summary>
        /// 添加仓储服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // 注册通用仓储(泛型)
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            
            // 注册工作单元
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            // 注册特定实体的仓储
            services.AddScoped<IUserRepository, UserRepository>();
            
            // 注册消息仓储
            services.AddScoped<IMessageRepository, MessageRepository>();
            
            // 这里可以继续添加其他实体的特定仓储...
            
            return services;
        }
    }
}