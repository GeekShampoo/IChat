using System;
using System.Collections.Generic;

namespace IChat.Protocol.Messages.Group
{
    /// <summary>
    /// 群组信息更新消息
    /// </summary>
    public class GroupInfoUpdatedMessage : BaseMessage
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 新的群组名称
        /// </summary>
        public string NewName { get; set; }

        /// <summary>
        /// 新的群组描述
        /// </summary>
        public string NewDescription { get; set; }

        /// <summary>
        /// 新的群组头像
        /// </summary>
        public string NewAvatarUrl { get; set; }

        /// <summary>
        /// 新的群组公告
        /// </summary>
        public string NewAnnouncement { get; set; }

        /// <summary>
        /// 更新者ID
        /// </summary>
        public Guid UpdatedByUserId { get; set; }

        /// <summary>
        /// 更新者名称
        /// </summary>
        public string UpdatedByUserName { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// 新群公告消息
    /// </summary>
    public class GroupAnnouncementUpdatedMessage : BaseMessage
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
        /// 新公告标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 新公告内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 发布人ID
        /// </summary>
        public Guid PublisherId { get; set; }

        /// <summary>
        /// 发布人名称
        /// </summary>
        public string PublisherName { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishedAt { get; set; }
    }

    /// <summary>
    /// 成员加入群组消息
    /// </summary>
    public class MemberJoinedGroupMessage : BaseMessage
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
        /// 加入的成员列表
        /// </summary>
        public List<GroupMemberInfo> JoinedMembers { get; set; } = new List<GroupMemberInfo>();

        /// <summary>
        /// 邀请人ID（如果是被邀请加入）
        /// </summary>
        public Guid? InviterId { get; set; }

        /// <summary>
        /// 邀请人名称
        /// </summary>
        public string InviterName { get; set; }

        /// <summary>
        /// 加入时间
        /// </summary>
        public DateTime JoinedAt { get; set; }
    }

    /// <summary>
    /// 成员离开群组消息
    /// </summary>
    public class MemberLeftGroupMessage : BaseMessage
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
        /// 离开的成员ID
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// 离开的成员名称
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 是否被移除（相对于自己退出）
        /// </summary>
        public bool IsRemoved { get; set; }

        /// <summary>
        /// 操作者ID（如果是被移除）
        /// </summary>
        public Guid? OperatorId { get; set; }

        /// <summary>
        /// 操作者名称
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 离开时间
        /// </summary>
        public DateTime LeftAt { get; set; }

        /// <summary>
        /// 移除原因（如果被移除）
        /// </summary>
        public string RemovalReason { get; set; }
    }

    /// <summary>
    /// 成员角色变更消息
    /// </summary>
    public class MemberRoleChangedMessage : BaseMessage
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
        /// 成员ID
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// 成员名称
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// 新角色
        /// </summary>
        public string NewRole { get; set; }

        /// <summary>
        /// 旧角色
        /// </summary>
        public string OldRole { get; set; }

        /// <summary>
        /// 操作者ID
        /// </summary>
        public Guid OperatorId { get; set; }

        /// <summary>
        /// 操作者名称
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 变更时间
        /// </summary>
        public DateTime ChangedAt { get; set; }
    }

    /// <summary>
    /// 群组解散消息
    /// </summary>
    public class GroupDisbandedMessage : BaseMessage
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
        /// 解散者ID
        /// </summary>
        public Guid DisbandedByUserId { get; set; }

        /// <summary>
        /// 解散者名称
        /// </summary>
        public string DisbandedByUserName { get; set; }

        /// <summary>
        /// 解散时间
        /// </summary>
        public DateTime DisbandedAt { get; set; }

        /// <summary>
        /// 解散原因
        /// </summary>
        public string Reason { get; set; }
    }

    /// <summary>
    /// 群组邀请消息
    /// </summary>
    public class GroupInvitationMessage : BaseMessage
    {
        /// <summary>
        /// 邀请ID
        /// </summary>
        public Guid InvitationId { get; set; }

        /// <summary>
        /// 群组ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 群组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 群组描述
        /// </summary>
        public string GroupDescription { get; set; }

        /// <summary>
        /// 群组头像
        /// </summary>
        public string GroupAvatar { get; set; }

        /// <summary>
        /// 邀请人ID
        /// </summary>
        public Guid InviterId { get; set; }

        /// <summary>
        /// 邀请人名称
        /// </summary>
        public string InviterName { get; set; }

        /// <summary>
        /// 邀请消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 邀请时间
        /// </summary>
        public DateTime InvitedAt { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// 当前成员数量
        /// </summary>
        public int MemberCount { get; set; }
    }

    /// <summary>
    /// 群组成员信息，用于群组相关消息
    /// </summary>
    public class GroupMemberInfo
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
        /// 成员昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 成员头像
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 群内显示名（群名片）
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 成员角色
        /// </summary>
        public string Role { get; set; }
    }
}