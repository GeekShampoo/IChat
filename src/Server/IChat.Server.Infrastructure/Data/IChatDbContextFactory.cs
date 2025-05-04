using IChat.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace IChat.Server.Infrastructure.Data
{
    /// <summary>
    /// 用于设计时创建数据库上下文的工厂类，使EF迁移工具能够找到并使用DbContext
    /// </summary>
    public class IChatDbContextFactory : IDesignTimeDbContextFactory<IChatDbContext>
    {
        public IChatDbContext CreateDbContext(string[] args)
        {
            // 获取Web项目的基础路径
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "IChat.Server.Web");
            
            // 获取配置
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // 创建DbContext选项
            var optionsBuilder = new DbContextOptionsBuilder<IChatDbContext>();
            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("IChat.Server.Infrastructure");
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });

            return new IChatDbContext(optionsBuilder.Options);
        }
    }
}