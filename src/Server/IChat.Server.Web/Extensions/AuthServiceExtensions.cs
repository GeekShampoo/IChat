using IChat.Server.Core.Interfaces;
using IChat.Server.Core.Models;
using IChat.Server.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace IChat.Server.Web.Extensions
{
    /// <summary>
    /// 认证服务扩展类
    /// </summary>
    public static class AuthServiceExtensions
    {
        /// <summary>
        /// 添加认证服务
        /// </summary>
        public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 配置JWT选项
            var jwtSection = configuration.GetSection("Jwt");
            services.Configure<JwtOptions>(jwtSection);
            
            var jwtOptions = jwtSection.Get<JwtOptions>();
            var key = Encoding.ASCII.GetBytes(jwtOptions.Secret);

            // 添加身份验证
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // 在生产环境中设置为true
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // 不允许任何时钟偏差
                };
                
                // 配置SignalR的身份验证
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        
                        // 如果请求来自SignalR Hub
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && 
                            (path.StartsWithSegments("/hubs/chat") || 
                             path.StartsWithSegments("/hubs/notification")))
                        {
                            // 设置token
                            context.Token = accessToken;
                        }
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };
            });

            // 注册认证服务
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}