using System;
using System.Collections.Generic;

namespace IChat.Protocol.Messages.System
{
    /// <summary>
    /// 服务器连接状态消息
    /// </summary>
    public class ConnectionStatusMessage : BaseMessage
    {
        /// <summary>
        /// 连接状态（Connected/Reconnecting/Disconnected）
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 连接ID
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// 服务器时间
        /// </summary>
        public DateTime ServerTime { get; set; }

        /// <summary>
        /// 连接详情信息
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// 重连倒计时（秒）
        /// </summary>
        public int? ReconnectCountdown { get; set; }
    }

    /// <summary>
    /// 服务器心跳消息
    /// </summary>
    public class HeartbeatMessage : BaseMessage
    {
        /// <summary>
        /// 服务器时间
        /// </summary>
        public DateTime ServerTime { get; set; }

        /// <summary>
        /// 心跳序号
        /// </summary>
        public long SequenceNumber { get; set; }
    }

    /// <summary>
    /// 错误消息
    /// </summary>
    public class ErrorMessage : BaseMessage
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// 相关上下文
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// 是否为致命错误
        /// </summary>
        public bool IsFatal { get; set; }

        /// <summary>
        /// 推荐操作
        /// </summary>
        public string RecommendedAction { get; set; }
    }

    /// <summary>
    /// 服务器维护消息
    /// </summary>
    public class ServerMaintenanceMessage : BaseMessage
    {
        /// <summary>
        /// 维护类型（Scheduled/Emergency/Ongoing）
        /// </summary>
        public string MaintenanceType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 预计结束时间
        /// </summary>
        public DateTime EstimatedEndTime { get; set; }

        /// <summary>
        /// 维护原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 影响的服务
        /// </summary>
        public List<string> AffectedServices { get; set; }

        /// <summary>
        /// 详细信息URL
        /// </summary>
        public string DetailsUrl { get; set; }
    }

    /// <summary>
    /// 客户端配置更新消息
    /// </summary>
    public class ClientConfigUpdateMessage : BaseMessage
    {
        /// <summary>
        /// 更新的配置项
        /// </summary>
        public Dictionary<string, string> ConfigItems { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 配置版本号
        /// </summary>
        public string ConfigVersion { get; set; }

        /// <summary>
        /// 是否需要立即应用
        /// </summary>
        public bool ApplyImmediately { get; set; }
    }

    /// <summary>
    /// 强制客户端操作消息
    /// </summary>
    public class ForceClientActionMessage : BaseMessage
    {
        /// <summary>
        /// 操作类型（Logout/ClearCache/RestartApp等）
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// 操作原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 是否立即执行
        /// </summary>
        public bool ExecuteImmediately { get; set; } = true;

        /// <summary>
        /// 延迟执行时间（秒）
        /// </summary>
        public int? DelaySeconds { get; set; }

        /// <summary>
        /// 操作参数（JSON格式）
        /// </summary>
        public string ActionParameters { get; set; }
    }
}