using System;
using IChat.Domain.Enums;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 好友关系实体，表示两个用户之间的好友关系
    /// </summary>
    public class Friendship : BaseEntity
    {
        /// <summary>
        /// 发起好友请求的用户ID
        /// </summary>
        public Guid InitiatorId { get; set; }

        /// <summary>
        /// 接收好友请求的用户ID
        /// </summary>
        public Guid RecipientId { get; set; }

        /// <summary>
        /// 好友关系状态
        /// </summary>
        public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;

        /// <summary>
        /// 好友请求消息
        /// </summary>
        public string RequestMessage { get; set; }

        /// <summary>
        /// 好友分组名称（发起者对接收者的分组）
        /// </summary>
        public string InitiatorGroupName { get; set; } = "我的好友";

        /// <summary>
        /// 好友分组名称（接收者对发起者的分组）
        /// </summary>
        public string RecipientGroupName { get; set; } = "我的好友";

        /// <summary>
        /// 发起者对接收者的备注名
        /// </summary>
        public string InitiatorRemark { get; set; }

        /// <summary>
        /// 接收者对发起者的备注名
        /// </summary>
        public string RecipientRemark { get; set; }

        /// <summary>
        /// 请求响应时间
        /// </summary>
        public DateTime? ResponseTime { get; set; }

        /// <summary>
        /// 发起好友请求的用户
        /// </summary>
        public virtual User Initiator { get; set; }

        /// <summary>
        /// 接收好友请求的用户
        /// </summary>
        public virtual User Recipient { get; set; }
    }
}