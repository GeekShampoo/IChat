namespace IChat.Domain.Enums
{
    /// <summary>
    /// 令牌类型枚举
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Bearer令牌
        /// </summary>
        Bearer = 0,
        
        /// <summary>
        /// JWT令牌
        /// </summary>
        Jwt = 1,
        
        /// <summary>
        /// 刷新令牌
        /// </summary>
        Refresh = 2
    }
}