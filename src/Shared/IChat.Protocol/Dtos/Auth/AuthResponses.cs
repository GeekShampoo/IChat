using System;
using IChat.Protocol.Contracts;

namespace IChat.Protocol.Dtos.Auth
{
    /// <summary>
    /// 用户认证响应
    /// </summary>
    public class AuthResponse : BaseResponse<AuthDataDto>
    {
    }

    /// <summary>
    /// 认证数据DTO
    /// </summary>
    public class AuthDataDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// 令牌类型
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// 访问令牌过期时间（Unix时间戳）
        /// </summary>
        public long ExpiresIn { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public string[] Roles { get; set; }

        /// <summary>
        /// 用户权限
        /// </summary>
        public string[] Permissions { get; set; }
    }

    /// <summary>
    /// 用户注册响应
    /// </summary>
    public class RegisterResponse : BaseResponse<RegisterResultDto>
    {
    }

    /// <summary>
    /// 注册结果DTO
    /// </summary>
    public class RegisterResultDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 是否需要邮箱验证
        /// </summary>
        public bool RequiresEmailVerification { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}