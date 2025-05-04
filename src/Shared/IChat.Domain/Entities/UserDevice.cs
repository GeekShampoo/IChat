using System;
using IChat.Domain.Enums;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 用户在线设备信息实体，用于记录用户的登录设备
    /// </summary>
    public class UserDevice : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType DeviceType { get; set; }

        /// <summary>
        /// 设备标识符（唯一）
        /// </summary>
        public string DeviceIdentifier { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 操作系统信息
        /// </summary>
        public string OperatingSystem { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// 推送令牌（用于移动设备推送通知）
        /// </summary>
        public string PushToken { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string LastLoginIp { get; set; }

        /// <summary>
        /// 最后活跃时间
        /// </summary>
        public DateTime LastActiveTime { get; set; }

        /// <summary>
        /// 设备状态
        /// </summary>
        public DeviceStatus Status { get; set; } = DeviceStatus.Online;

        /// <summary>
        /// 关联的用户
        /// </summary>
        public virtual User User { get; set; }
    }
}