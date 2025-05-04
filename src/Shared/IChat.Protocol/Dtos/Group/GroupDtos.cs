using System;
using System.Collections.Generic;
using IChat.Protocol.Contracts;

namespace IChat.Protocol.Dtos.Group
{
    /// <summary>
    /// 群组信息DTO
    /// </summary>
    public class GroupDto
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 群组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 群组描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 群组头像URL
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 群组公告
        /// </summary>
        public string Announcement { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid CreatorId { get; set; }

        /// <summary>
        /// 创建者用户名
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 群组成员数量
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        /// 最大成员数量
        /// </summary>
        public int MaxMemberCount { get; set; }

        /// <summary>
        /// 是否需要验证加入
        /// </summary>
        public bool RequiresApproval { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 当前用户在群组中的角色
        /// </summary>
        public string CurrentUserRole { get; set; }
    }

    /// <summary>
    /// 群组详细信息DTO
    /// </summary>
    public class GroupDetailDto : GroupDto
    {
        /// <summary>
        /// 群组成员列表
        /// </summary>
        public List<GroupMemberDto> Members { get; set; } = new List<GroupMemberDto>();
    }

    /// <summary>
    /// 群组成员DTO
    /// </summary>
    public class GroupMemberDto
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
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 群内显示名称（群名片）
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 成员角色
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 加入时间
        /// </summary>
        public DateTime JoinTime { get; set; }

        /// <summary>
        /// 最后发言时间
        /// </summary>
        public DateTime? LastActiveTime { get; set; }

        /// <summary>
        /// 邀请人ID（如果是被邀请加入的）
        /// </summary>
        public Guid? InviterId { get; set; }

        /// <summary>
        /// 邀请人名称
        /// </summary>
        public string InviterName { get; set; }
    }

    /// <summary>
    /// 创建群组请求
    /// </summary>
    public class CreateGroupRequest : BaseRequest
    {
        /// <summary>
        /// 群组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 群组描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 群组头像URL
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 群组公告
        /// </summary>
        public string Announcement { get; set; }

        /// <summary>
        /// 最大成员数量
        /// </summary>
        public int MaxMemberCount { get; set; } = 200;

        /// <summary>
        /// 是否需要验证加入
        /// </summary>
        public bool RequiresApproval { get; set; } = true;

        /// <summary>
        /// 是否允许群成员邀请他人
        /// </summary>
        public bool AllowMemberInvitation { get; set; } = false;

        /// <summary>
        /// 初始邀请的成员ID列表
        /// </summary>
        public List<Guid> InvitedMembers { get; set; } = new List<Guid>();
    }

    /// <summary>
    /// 更新群组信息请求
    /// </summary>
    public class UpdateGroupRequest : BaseRequest
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 群组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 群组描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 群组头像URL
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 群组公告
        /// </summary>
        public string Announcement { get; set; }

        /// <summary>
        /// 是否需要验证加入
        /// </summary>
        public bool? RequiresApproval { get; set; }

        /// <summary>
        /// 是否允许群成员邀请他人
        /// </summary>
        public bool? AllowMemberInvitation { get; set; }
    }

    /// <summary>
    /// 加入群组请求
    /// </summary>
    public class JoinGroupRequest : BaseRequest
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 申请消息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 邀请加入群组请求
    /// </summary>
    public class InviteToGroupRequest : BaseRequest
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 被邀请用户ID列表
        /// </summary>
        public List<Guid> UserIds { get; set; } = new List<Guid>();

        /// <summary>
        /// 邀请消息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 处理入群申请请求
    /// </summary>
    public class HandleGroupJoinRequest : BaseRequest
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// 是否接受
        /// </summary>
        public bool Accept { get; set; }

        /// <summary>
        /// 回复消息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 退出群组请求
    /// </summary>
    public class LeaveGroupRequest : BaseRequest
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        public Guid GroupId { get; set; }
    }

    /// <summary>
    /// 移除群成员请求
    /// </summary>
    public class RemoveGroupMemberRequest : BaseRequest
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 要移除的成员ID
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// 移除原因
        /// </summary>
        public string Reason { get; set; }
    }

    /// <summary>
    /// 更新群成员角色请求
    /// </summary>
    public class UpdateGroupMemberRoleRequest : BaseRequest
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 成员ID
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// 新角色
        /// </summary>
        public string NewRole { get; set; }
    }

    /// <summary>
    /// 更新群成员显示名称请求
    /// </summary>
    public class UpdateGroupMemberDisplayNameRequest : BaseRequest
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 成员ID
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// 新显示名称（群名片）
        /// </summary>
        public string DisplayName { get; set; }
    }

    /// <summary>
    /// 解散群组请求
    /// </summary>
    public class DisbandGroupRequest : BaseRequest
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 确认码（群组名称，用于双重验证）
        /// </summary>
        public string ConfirmationCode { get; set; }
    }
}