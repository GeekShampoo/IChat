namespace IChat.Domain.Enums
{
    /// <summary>
    /// 用户在线状态枚举
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// 离线状态
        /// </summary>
        Offline = 0,
        
        /// <summary>
        /// 在线状态
        /// </summary>
        Online = 1,
        
        /// <summary>
        /// 忙碌状态
        /// </summary>
        Busy = 2,
        
        /// <summary>
        /// 离开状态
        /// </summary>
        Away = 3,
        
        /// <summary>
        /// 隐身状态
        /// </summary>
        Invisible = 4
    }
}