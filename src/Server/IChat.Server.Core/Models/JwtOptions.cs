namespace IChat.Server.Core.Models
{
    /// <summary>
    /// JWT 配置选项
    /// </summary>
    public class JwtOptions
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 受众
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 访问令牌过期时间（分钟）
        /// </summary>
        public int AccessTokenExpirationMinutes { get; set; } = 30;

        /// <summary>
        /// 刷新令牌过期时间（天）
        /// </summary>
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}