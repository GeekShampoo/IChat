using System;

namespace IChat.Protocol.Messages.Friend
{
    /// <summary>
    /// 收到好友请求消息
    /// </summary>
    public class FriendRequestReceivedMessage : BaseMessage
    {
        /// <summary>
        /// 好友请求ID
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// 发起人ID
        /// </summary>
        public Guid InitiatorId { get; set; }

        /// <summary>
        /// 发起人用户名
        /// </summary>
        public string InitiatorUsername { get; set; }

        /// <summary>
        /// 发起人昵称
        /// </summary>
        public string InitiatorNickname { get; set; }

        /// <summary>
        /// 发起人头像
        /// </summary>
        public string InitiatorAvatar { get; set; }

        /// <summary>
        /// 请求消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 好友请求状态更新消息
    /// </summary>
    public class FriendRequestStatusUpdatedMessage : BaseMessage
    {
        /// <summary>
        /// 好友请求ID
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// 新状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 对方用户ID
        /// </summary>
        public Guid OtherUserId { get; set; }

        /// <summary>
        /// 对方用户名
        /// </summary>
        public string OtherUsername { get; set; }

        /// <summary>
        /// 对方昵称
        /// </summary>
        public string OtherNickname { get; set; }

        /// <summary>
        /// 对方头像
        /// </summary>
        public string OtherAvatar { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 回复消息（如果有）
        /// </summary>
        public string ResponseMessage { get; set; }
    }

    /// <summary>
    /// 好友关系变更消息
    /// </summary>
    public class FriendshipChangedMessage : BaseMessage
    {
        /// <summary>
        /// 好友ID
        /// </summary>
        public Guid FriendId { get; set; }

        /// <summary>
        /// 好友用户名
        /// </summary>
        public string FriendUsername { get; set; }

        /// <summary>
        /// 好友昵称
        /// </summary>
        public string FriendNickname { get; set; }

        /// <summary>
        /// 变更类型（Added/Updated/Removed）
        /// </summary>
        public string ChangeType { get; set; }

        /// <summary>
        /// 变更时间
        /// </summary>
        public DateTime ChangedAt { get; set; }
    }
}