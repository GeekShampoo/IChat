namespace IChat.Domain.Enums
{
    /// <summary>
    /// 设备类型枚举
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// 未知设备
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// PC桌面端
        /// </summary>
        Desktop = 1,
        
        /// <summary>
        /// Web网页端
        /// </summary>
        Web = 2,
        
        /// <summary>
        /// 移动设备 - iOS
        /// </summary>
        IOS = 3,
        
        /// <summary>
        /// 移动设备 - Android
        /// </summary>
        Android = 4,
        
        /// <summary>
        /// 小程序
        /// </summary>
        MiniProgram = 5
    }
}