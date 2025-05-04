using System;

namespace IChat.Protocol.Messages
{
    /// <summary>
    /// 表示通过SignalR传输的基础消息
    /// </summary>
    public abstract class BaseMessage
    {
        /// <summary>
        /// 消息唯一标识符
        /// </summary>
        public Guid MessageId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 消息发送时间
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 消息类型（派生类的名称）
        /// </summary>
        public string MessageType => GetType().Name;
    }
}