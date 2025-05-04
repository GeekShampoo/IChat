using System;
using System.Collections.Generic;

namespace IChat.Protocol.Dtos.Message
{
    /// <summary>
    /// 消息DTO
    /// </summary>
    public class MessageDto
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 发送者ID
        /// </summary>
        public Guid SenderId { get; set; }

        /// <summary>
        /// 发送者名称
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 发送者头像
        /// </summary>
        public string SenderAvatar { get; set; }

        /// <summary>
        /// 接收者ID（私聊）
        /// </summary>
        public Guid? RecipientId { get; set; }

        /// <summary>
        /// 群组ID（群聊）
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 引用回复的消息ID
        /// </summary>
        public Guid? ReplyToMessageId { get; set; }

        /// <summary>
        /// 引用的消息（如果有）
        /// </summary>
        public MessageReferenceDto ReplyToMessage { get; set; }

        /// <summary>
        /// 消息状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; }

        /// <summary>
        /// 送达时间
        /// </summary>
        public DateTime? DeliveredTime { get; set; }

        /// <summary>
        /// 已读时间
        /// </summary>
        public DateTime? ReadTime { get; set; }

        /// <summary>
        /// 已读回执信息
        /// </summary>
        public List<MessageReadReceiptDto> ReadReceipts { get; set; }

        /// <summary>
        /// 附加数据（JSON格式）
        /// </summary>
        public string ExtendedData { get; set; }
    }

    /// <summary>
    /// 消息引用DTO（用于消息引用）
    /// </summary>
    public class MessageReferenceDto
    {
        /// <summary>
        /// 被引用消息ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 被引用消息发送者ID
        /// </summary>
        public Guid SenderId { get; set; }

        /// <summary>
        /// 被引用消息发送者名称
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 被引用消息类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 被引用消息内容预览（截断的内容）
        /// </summary>
        public string ContentPreview { get; set; }
    }

    /// <summary>
    /// 消息已读回执DTO
    /// </summary>
    public class MessageReadReceiptDto
    {
        /// <summary>
        /// 已读用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 已读用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 已读时间
        /// </summary>
        public DateTime ReadAt { get; set; }
    }

    /// <summary>
    /// 发送消息请求
    /// </summary>
    public class SendMessageRequest
    {
        /// <summary>
        /// 客户端消息ID（用于确认）
        /// </summary>
        public Guid ClientMessageId { get; set; }

        /// <summary>
        /// 接收者ID（私聊）
        /// </summary>
        public Guid? RecipientId { get; set; }

        /// <summary>
        /// 群组ID（群聊）
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string Type { get; set; } = "Text";

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 回复消息ID
        /// </summary>
        public Guid? ReplyToMessageId { get; set; }

        /// <summary>
        /// 附加数据（JSON格式）
        /// </summary>
        public string ExtendedData { get; set; }
    }

    /// <summary>
    /// 获取历史消息请求
    /// </summary>
    public class GetHistoryMessagesRequest
    {
        /// <summary>
        /// 对话类型 (Private/Group)
        /// </summary>
        public string ConversationType { get; set; }

        /// <summary>
        /// 对话目标ID (用户ID或群组ID)
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// 分页令牌 (用于获取下一页)
        /// </summary>
        public string PageToken { get; set; }

        /// <summary>
        /// 每页消息数量
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 获取方向 (更早的消息/更新的消息)
        /// </summary>
        public string Direction { get; set; } = "Backward";
    }

    /// <summary>
    /// 消息已读通知
    /// </summary>
    public class MessageReadNotification
    {
        /// <summary>
        /// 被标记为已读的消息ID
        /// </summary>
        public List<Guid> MessageIds { get; set; } = new List<Guid>();

        /// <summary>
        /// 对话类型 (Private/Group)
        /// </summary>
        public string ConversationType { get; set; }

        /// <summary>
        /// 对话目标ID (用户ID或群组ID)
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// 已读时间
        /// </summary>
        public DateTime ReadAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// 消息撤回请求
    /// </summary>
    public class RecallMessageRequest
    {
        /// <summary>
        /// 要撤回的消息ID
        /// </summary>
        public Guid MessageId { get; set; }
    }
}