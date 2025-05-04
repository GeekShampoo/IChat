using System;
using IChat.Domain.Enums;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 会话实体，表示用户的聊天会话
    /// </summary>
    public class Conversation : BaseEntity
    {
        /// <summary>
        /// 会话所有者ID
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 会话类型
        /// </summary>
        public ConversationType Type { get; set; }

        /// <summary>
        /// 目标ID（私聊时为用户ID，群聊时为群组ID）
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// 最后一条消息ID
        /// </summary>
        public Guid? LastMessageId { get; set; }

        /// <summary>
        /// 未读消息数量
        /// </summary>
        public int UnreadCount { get; set; } = 0;

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsPinned { get; set; } = false;

        /// <summary>
        /// 是否静音
        /// </summary>
        public bool IsMuted { get; set; } = false;

        /// <summary>
        /// 是否显示通知
        /// </summary>
        public bool ShowNotification { get; set; } = true;

        /// <summary>
        /// 最后一次阅读时间
        /// </summary>
        public DateTime? LastReadTime { get; set; }

        /// <summary>
        /// 会话拥有者
        /// </summary>
        public virtual User Owner { get; set; }

        /// <summary>
        /// 最后一条消息
        /// </summary>
        public virtual Message LastMessage { get; set; }
    }
}