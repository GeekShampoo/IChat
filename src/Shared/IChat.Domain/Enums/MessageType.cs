namespace IChat.Domain.Enums
{
    /// <summary>
    /// 消息类型枚举
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 文本消息
        /// </summary>
        Text = 0,
        
        /// <summary>
        /// 图片消息
        /// </summary>
        Image = 1,
        
        /// <summary>
        /// 语音消息
        /// </summary>
        Voice = 2,
        
        /// <summary>
        /// 视频消息
        /// </summary>
        Video = 3,
        
        /// <summary>
        /// 文件消息
        /// </summary>
        File = 4,
        
        /// <summary>
        /// 位置消息
        /// </summary>
        Location = 5,
        
        /// <summary>
        /// 系统通知消息
        /// </summary>
        SystemNotification = 6,
        
        /// <summary>
        /// 自定义消息
        /// </summary>
        Custom = 7
    }
}