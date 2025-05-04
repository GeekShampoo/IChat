namespace IChat.Domain.Enums
{
    /// <summary>
    /// 好友关系状态枚举
    /// </summary>
    public enum FriendshipStatus
    {
        /// <summary>
        /// 已发送请求，等待接受
        /// </summary>
        Pending = 0,
        
        /// <summary>
        /// 已接受好友请求
        /// </summary>
        Accepted = 1,
        
        /// <summary>
        /// 已拒绝好友请求
        /// </summary>
        Rejected = 2,
        
        /// <summary>
        /// 已屏蔽
        /// </summary>
        Blocked = 3
    }
}