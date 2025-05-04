namespace IChat.Domain.Enums
{
    /// <summary>
    /// 通知类型枚举
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// 系统通知
        /// </summary>
        System = 0,
        
        /// <summary>
        /// 好友请求通知
        /// </summary>
        FriendRequest = 1,
        
        /// <summary>
        /// 好友接受通知
        /// </summary>
        FriendAccepted = 2,
        
        /// <summary>
        /// 群组邀请通知
        /// </summary>
        GroupInvitation = 3,
        
        /// <summary>
        /// 群组加入请求通知
        /// </summary>
        GroupJoinRequest = 4,
        
        /// <summary>
        /// 群组加入通知
        /// </summary>
        GroupJoined = 5,
        
        /// <summary>
        /// 新消息通知
        /// </summary>
        NewMessage = 6,
        
        /// <summary>
        /// 消息提醒通知
        /// </summary>
        MessageReminder = 7,
        
        /// <summary>
        /// 账号通知（如登录提醒等）
        /// </summary>
        Account = 8
    }
}