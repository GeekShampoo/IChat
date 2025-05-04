namespace IChat.Domain.Enums
{
    /// <summary>
    /// 用户角色枚举
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        User = 0,
        
        /// <summary>
        /// 管理员
        /// </summary>
        Admin = 1,
        
        /// <summary>
        /// 系统管理员
        /// </summary>
        SystemAdmin = 2
    }
}