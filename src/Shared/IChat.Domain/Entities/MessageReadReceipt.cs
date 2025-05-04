using System;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 消息已读回执实体，用于跟踪群组消息的已读状态
    /// </summary>
    public class MessageReadReceipt : BaseEntity
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// 读取消息的用户ID
        /// </summary>
        public Guid ReadByUserId { get; set; }

        /// <summary>
        /// 读取时间
        /// </summary>
        public DateTime ReadAt { get; set; }

        /// <summary>
        /// 相关消息
        /// </summary>
        public virtual Message Message { get; set; }

        /// <summary>
        /// 读取消息的用户
        /// </summary>
        public virtual User ReadByUser { get; set; }
    }
}