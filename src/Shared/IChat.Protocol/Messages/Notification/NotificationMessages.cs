using System;

namespace IChat.Protocol.Messages.Notification
{
    /// <summary>
    /// 通知消息基类
    /// </summary>
    public class NotificationMessage : BaseMessage
    {
        /// <summary>
        /// 通知ID
        /// </summary>
        public Guid NotificationId { get; set; }

        /// <summary>
        /// 通知类型
        /// </summary>
        public string NotificationType { get; set; }

        /// <summary>
        /// 通知标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 通知内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 相关ID（如消息ID、好友请求ID等）
        /// </summary>
        public Guid? RelatedId { get; set; }

        /// <summary>
        /// 通知URL链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 通知时间
        /// </summary>
        public DateTime NotificationTime { get; set; }

        /// <summary>
        /// 额外数据（JSON格式）
        /// </summary>
        public string Data { get; set; }
    }

    /// <summary>
    /// 系统通知消息
    /// </summary>
    public class SystemNotificationMessage : NotificationMessage
    {
        /// <summary>
        /// 系统通知级别（Info/Warning/Error）
        /// </summary>
        public string Level { get; set; } = "Info";

        /// <summary>
        /// 是否需要确认
        /// </summary>
        public bool RequiresConfirmation { get; set; } = false;

        /// <summary>
        /// 过期时间（如果有）
        /// </summary>
        public DateTime? ExpiresAt { get; set; }
    }

    /// <summary>
    /// 新设备登录通知消息
    /// </summary>
    public class NewDeviceLoginMessage : NotificationMessage
    {
        /// <summary>
        /// 登录设备信息
        /// </summary>
        public string DeviceInfo { get; set; }

        /// <summary>
        /// 登录IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 登录位置
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 是否为异常登录
        /// </summary>
        public bool IsUnusualLogin { get; set; }
    }

    /// <summary>
    /// 账号安全通知消息
    /// </summary>
    public class AccountSecurityMessage : NotificationMessage
    {
        /// <summary>
        /// 安全事件类型
        /// </summary>
        public string SecurityEventType { get; set; }

        /// <summary>
        /// 事件时间
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// 是否需要用户处理
        /// </summary>
        public bool RequiresAction { get; set; }

        /// <summary>
        /// 建议操作
        /// </summary>
        public string SuggestedAction { get; set; }
    }

    /// <summary>
    /// 应用更新通知消息
    /// </summary>
    public class AppUpdateMessage : NotificationMessage
    {
        /// <summary>
        /// 新版本号
        /// </summary>
        public string NewVersion { get; set; }

        /// <summary>
        /// 更新类型（Optional/Recommended/Required）
        /// </summary>
        public string UpdateType { get; set; }

        /// <summary>
        /// 更新内容摘要
        /// </summary>
        public string UpdateSummary { get; set; }

        /// <summary>
        /// 下载链接
        /// </summary>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// 是否强制更新
        /// </summary>
        public bool IsForced { get; set; }

        /// <summary>
        /// 更新截止日期（如果有）
        /// </summary>
        public DateTime? Deadline { get; set; }
    }
}