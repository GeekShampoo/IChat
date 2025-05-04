using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace IChat.Server.Web.Extensions
{
    /// <summary>
    /// Swagger/OpenAPI 配置扩展类
    /// </summary>
    public static class OpenApiExtensions
    {
        /// <summary>
        /// 添加 OpenAPI/Swagger 服务
        /// </summary>
        public static IServiceCollection AddOpenApi(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "IChat API",
                    Version = "v1",
                    Description = "IChat 系统 API 接口文档",
                    Contact = new OpenApiContact
                    {
                        Name = "IChat Team",
                        Email = "contact@ichat.example.com"
                    }
                });

                // 添加JWT Bearer认证配置
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // 启用XML注释
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

        /// <summary>
        /// 配置 OpenAPI/Swagger 中间件
        /// </summary>
        public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "IChat API v1");
                options.RoutePrefix = "swagger";
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                options.DefaultModelsExpandDepth(-1); // 隐藏Models
            });

            return app;
        }
    }
}