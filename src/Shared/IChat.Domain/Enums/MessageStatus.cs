namespace IChat.Domain.Enums
{
    /// <summary>
    /// 消息状态枚举
    /// </summary>
    public enum MessageStatus
    {
        /// <summary>
        /// 发送中
        /// </summary>
        Sending = 0,
        
        /// <summary>
        /// 已发送
        /// </summary>
        Sent = 1,
        
        /// <summary>
        /// 已送达
        /// </summary>
        Delivered = 2,
        
        /// <summary>
        /// 已读
        /// </summary>
        Read = 3,
        
        /// <summary>
        /// 发送失败
        /// </summary>
        Failed = 4,
        
        /// <summary>
        /// 已撤回
        /// </summary>
        Recalled = 5
    }
}