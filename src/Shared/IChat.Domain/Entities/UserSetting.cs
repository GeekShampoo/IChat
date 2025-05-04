using System;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 用户设置实体，存储用户的各种应用偏好
    /// </summary>
    public class UserSetting : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 主题模式（0: 跟随系统, 1: 浅色, 2: 深色）
        /// </summary>
        public int ThemeMode { get; set; } = 0;

        /// <summary>
        /// 主题颜色（十六进制颜色值）
        /// </summary>
        public string ThemeColor { get; set; } = "#1890ff";

        /// <summary>
        /// 是否启用声音提醒
        /// </summary>
        public bool EnableSoundNotification { get; set; } = true;

        /// <summary>
        /// 是否显示通知
        /// </summary>
        public bool EnableNotification { get; set; } = true;

        /// <summary>
        /// 是否在通知中显示消息预览
        /// </summary>
        public bool ShowMessagePreview { get; set; } = true;

        /// <summary>
        /// 新消息通知声音
        /// </summary>
        public string NotificationSound { get; set; } = "default";

        /// <summary>
        /// 是否自动下载接收的文件
        /// </summary>
        public bool AutoDownloadFiles { get; set; } = false;

        /// <summary>
        /// 自动下载的最大文件大小(MB)
        /// </summary>
        public int AutoDownloadMaxSize { get; set; } = 10;

        /// <summary>
        /// 字体大小（0: 小, 1: 中, 2: 大）
        /// </summary>
        public int FontSize { get; set; } = 1;

        /// <summary>
        /// 语言设置（如zh-CN, en-US）
        /// </summary>
        public string Language { get; set; } = "zh-CN";

        /// <summary>
        /// 是否在启动时自动登录
        /// </summary>
        public bool AutoLogin { get; set; } = true;

        /// <summary>
        /// 是否在其他设备上线时接收通知
        /// </summary>
        public bool NotifyOnOtherDeviceLogin { get; set; } = true;

        /// <summary>
        /// 关联的用户
        /// </summary>
        public virtual User User { get; set; }
    }
}