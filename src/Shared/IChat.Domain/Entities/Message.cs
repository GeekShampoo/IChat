using System;
using System.Collections.Generic;
using IChat.Domain.Enums;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 消息实体，表示用户之间或在群组中发送的消息
    /// </summary>
    public class Message : BaseEntity
    {
        /// <summary>
        /// 发送者用户ID
        /// </summary>
        public Guid SenderId { get; set; }

        /// <summary>
        /// 接收者用户ID（私聊消息）
        /// </summary>
        public Guid? RecipientId { get; set; }

        /// <summary>
        /// 群组ID（群聊消息）
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType Type { get; set; } = MessageType.Text;

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 原始消息ID（用于引用回复）
        /// </summary>
        public Guid? ReplyToMessageId { get; set; }

        /// <summary>
        /// 消息状态
        /// </summary>
        public MessageStatus Status { get; set; } = MessageStatus.Sending;

        /// <summary>
        /// 消息发送时间
        /// </summary>
        public DateTime SendTime { get; set; }

        /// <summary>
        /// 消息送达时间
        /// </summary>
        public DateTime? DeliveredTime { get; set; }

        /// <summary>
        /// 消息已读时间
        /// </summary>
        public DateTime? ReadTime { get; set; }

        /// <summary>
        /// 引用的原始消息（用于回复功能）
        /// </summary>
        public virtual Message ReplyToMessage { get; set; }

        /// <summary>
        /// 引用本消息的消息列表
        /// </summary>
        public virtual ICollection<Message> Replies { get; set; }

        /// <summary>
        /// 附加数据（JSON格式，用于存储不同消息类型的额外信息）
        /// </summary>
        public string ExtendedData { get; set; }

        /// <summary>
        /// 消息发送者
        /// </summary>
        public virtual User Sender { get; set; }

        /// <summary>
        /// 消息接收者（私聊）
        /// </summary>
        public virtual User Recipient { get; set; }

        /// <summary>
        /// 消息所属群组（群聊）
        /// </summary>
        public virtual Group Group { get; set; }

        /// <summary>
        /// 已读回执集合（针对群消息）
        /// </summary>
        public virtual ICollection<MessageReadReceipt> ReadReceipts { get; set; }

        public Message()
        {
            Replies = new List<Message>();
            ReadReceipts = new List<MessageReadReceipt>();
        }
    }
}