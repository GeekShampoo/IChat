using System;
using System.Collections.Generic;

namespace IChat.Protocol.Messages.Presence
{
    /// <summary>
    /// 用户在线状态更新消息
    /// </summary>
    public class UserPresenceUpdatedMessage : BaseMessage
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 新的在线状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 自定义状态消息
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// 最后活跃时间
        /// </summary>
        public DateTime LastActiveTime { get; set; }

        /// <summary>
        /// 设备信息
        /// </summary>
        public string DeviceInfo { get; set; }
    }

    /// <summary>
    /// 好友在线状态批量更新消息
    /// </summary>
    public class FriendsPresenceMessage : BaseMessage
    {
        /// <summary>
        /// 在线好友状态列表
        /// </summary>
        public List<FriendPresenceInfo> OnlineFriends { get; set; } = new List<FriendPresenceInfo>();
    }

    /// <summary>
    /// 好友在线状态信息
    /// </summary>
    public class FriendPresenceInfo
    {
        /// <summary>
        /// 好友ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 好友用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 在线状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 自定义状态消息
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// 最后活跃时间
        /// </summary>
        public DateTime LastActiveTime { get; set; }

        /// <summary>
        /// 最后可见设备类型
        /// </summary>
        public string DeviceType { get; set; }
    }

    /// <summary>
    /// 群组成员在线状态批量更新消息
    /// </summary>
    public class GroupMembersPresenceMessage : BaseMessage
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 群组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 在线成员状态列表
        /// </summary>
        public List<GroupMemberPresenceInfo> OnlineMembers { get; set; } = new List<GroupMemberPresenceInfo>();
    }

    /// <summary>
    /// 群组成员在线状态信息
    /// </summary>
    public class GroupMemberPresenceInfo
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 成员用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 成员在群组中的显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 在线状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 最后活跃时间
        /// </summary>
        public DateTime LastActiveTime { get; set; }
    }
}