namespace IChat.Domain.Enums
{
    /// <summary>
    /// 会话类型枚举
    /// </summary>
    public enum ConversationType
    {
        /// <summary>
        /// 私聊会话
        /// </summary>
        Private = 0,
        
        /// <summary>
        /// 群聊会话
        /// </summary>
        Group = 1,
        
        /// <summary>
        /// 系统通知会话
        /// </summary>
        System = 2
    }
}