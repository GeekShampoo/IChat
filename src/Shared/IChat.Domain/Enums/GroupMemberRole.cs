namespace IChat.Domain.Enums
{
    /// <summary>
    /// 群组成员角色枚举
    /// </summary>
    public enum GroupMemberRole
    {
        /// <summary>
        /// 普通成员
        /// </summary>
        Member = 0,
        
        /// <summary>
        /// 管理员
        /// </summary>
        Admin = 1,
        
        /// <summary>
        /// 群主
        /// </summary>
        Owner = 2
    }
}