using System;
using IChat.Domain.Enums;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 通知实体，表示发送给用户的各类通知
    /// </summary>
    public class Notification : BaseEntity
    {
        /// <summary>
        /// 通知接收者ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 通知类型
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        /// 通知标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 通知内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 通知数据（JSON格式，用于存储额外信息）
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 相关ID（如消息ID、好友请求ID等）
        /// </summary>
        public Guid? RelatedId { get; set; }

        /// <summary>
        /// 通知URL链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// 阅读时间
        /// </summary>
        public DateTime? ReadAt { get; set; }

        /// <summary>
        /// 通知接收者
        /// </summary>
        public virtual User User { get; set; }
    }
}