using IChat.Server.Core.Interfaces;
using IChat.Server.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IChat.Server.Core.Extensions
{
    /// <summary>
    /// 核心业务服务注册扩展类
    /// </summary>
    public static class CoreServicesExtensions
    {
        /// <summary>
        /// 添加核心业务服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            // 添加消息服务
            services.AddScoped<IMessageService, MessageService>();
            
            // 这里可以继续添加其他核心业务服务...
            
            return services;
        }
    }
}