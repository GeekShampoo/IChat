using System;

namespace IChat.Protocol.Contracts
{
    /// <summary>
    /// 表示客户端到服务器的基础请求
    /// </summary>
    public abstract class BaseRequest
    {
        /// <summary>
        /// 请求ID，用于跟踪和关联请求与响应
        /// </summary>
        public Guid RequestId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 发送请求的时间戳
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 客户端版本信息
        /// </summary>
        public string ClientVersion { get; set; }
    }
}