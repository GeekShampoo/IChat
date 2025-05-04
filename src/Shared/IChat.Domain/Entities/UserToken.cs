using System;
using IChat.Domain.Enums;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 用户令牌实体，用于管理用户登录凭证
    /// </summary>
    public class UserToken : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

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
        public TokenType TokenType { get; set; } = TokenType.Bearer;

        /// <summary>
        /// 访问令牌过期时间
        /// </summary>
        public DateTime AccessTokenExpiresAt { get; set; }

        /// <summary>
        /// 刷新令牌过期时间
        /// </summary>
        public DateTime RefreshTokenExpiresAt { get; set; }

        /// <summary>
        /// 创建令牌的设备ID
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// 创建令牌时的IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 是否已被撤销
        /// </summary>
        public bool IsRevoked { get; set; } = false;

        /// <summary>
        /// 撤销时间
        /// </summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// 令牌所属用户
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// 关联的设备
        /// </summary>
        public virtual UserDevice Device { get; set; }
    }
}