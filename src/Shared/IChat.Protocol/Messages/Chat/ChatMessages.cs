using System;
using System.Collections.Generic;
using IChat.Protocol.Dtos.Message;

namespace IChat.Protocol.Messages.Chat
{
    /// <summary>
    /// 聊天消息，用于实时传递消息内容
    /// </summary>
    public class ChatMessageReceivedMessage : BaseMessage
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        public MessageDto Message { get; set; }
    }

    /// <summary>
    /// 消息状态更新，用于通知消息状态变化
    /// </summary>
    public class MessageStatusUpdatedMessage : BaseMessage
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// 客户端消息ID（用于客户端关联）
        /// </summary>
        public Guid? ClientMessageId { get; set; }

        /// <summary>
        /// 新状态
        /// </summary>
        public string NewStatus { get; set; }

        /// <summary>
        /// 状态更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 相关用户ID（如已读用户）
        /// </summary>
        public Guid? RelatedUserId { get; set; }
    }

    /// <summary>
    /// 消息撤回通知
    /// </summary>
    public class MessageRecalledMessage : BaseMessage
    {
        /// <summary>
        /// 被撤回的消息ID
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// 会话类型
        /// </summary>
        public string ConversationType { get; set; }

        /// <summary>
        /// 会话目标ID
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        public Guid OperatorId { get; set; }

        /// <summary>
        /// 操作者名称
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 撤回时间
        /// </summary>
        public DateTime RecalledAt { get; set; }
    }

    /// <summary>
    /// 消息已读通知
    /// </summary>
    public class MessagesReadMessage : BaseMessage
    {
        /// <summary>
        /// 已读用户ID
        /// </summary>
        public Guid ReadByUserId { get; set; }

        /// <summary>
        /// 已读用户名称
        /// </summary>
        public string ReadByUserName { get; set; }

        /// <summary>
        /// 已读消息ID列表
        /// </summary>
        public List<Guid> MessageIds { get; set; } = new List<Guid>();

        /// <summary>
        /// 会话类型
        /// </summary>
        public string ConversationType { get; set; }

        /// <summary>
        /// 会话目标ID
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// 已读时间
        /// </summary>
        public DateTime ReadAt { get; set; }
    }

    /// <summary>
    /// 收到新消息通知（简短通知，不含完整消息内容）
    /// </summary>
    public class NewMessageNotificationMessage : BaseMessage
    {
        /// <summary>
        /// 发送者ID
        /// </summary>
        public Guid SenderId { get; set; }

        /// <summary>
        /// 发送者名称
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 会话类型
        /// </summary>
        public string ConversationType { get; set; }

        /// <summary>
        /// 会话目标ID（私聊对方ID或群组ID）
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// 会话目标名称
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// 消息预览（截断的内容）
        /// </summary>
        public string MessagePreview { get; set; }

        /// <summary>
        /// 消息发送时间
        /// </summary>
        public DateTime SentAt { get; set; }

        /// <summary>
        /// 未读消息数量
        /// </summary>
        public int UnreadCount { get; set; }
    }

    /// <summary>
    /// 正在输入状态通知
    /// </summary>
    public class TypingNotificationMessage : BaseMessage
    {
        /// <summary>
        /// 正在输入的用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 会话类型
        /// </summary>
        public string ConversationType { get; set; }

        /// <summary>
        /// 会话目标ID
        /// </summary>
        public Guid TargetId { get; set; }
    }
}