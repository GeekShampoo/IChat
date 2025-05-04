namespace IChat.Domain.Enums
{
    /// <summary>
    /// 设备状态枚举
    /// </summary>
    public enum DeviceStatus
    {
        /// <summary>
        /// 在线状态
        /// </summary>
        Online = 0,
        
        /// <summary>
        /// 离线状态
        /// </summary>
        Offline = 1,
        
        /// <summary>
        /// 已锁定
        /// </summary>
        Locked = 2,
        
        /// <summary>
        /// 已禁用
        /// </summary>
        Disabled = 3
    }
}