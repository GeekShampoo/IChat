using System;
using IChat.Domain.Enums;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 群组成员实体，表示用户与群组的关系
    /// </summary>
    public class GroupMember : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 群组ID
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// 群组中的昵称（群名片）
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 成员角色
        /// </summary>
        public GroupMemberRole Role { get; set; } = GroupMemberRole.Member;

        /// <summary>
        /// 加入时间
        /// </summary>
        public DateTime JoinTime { get; set; }

        /// <summary>
        /// 邀请人ID（如果是被邀请加入的）
        /// </summary>
        public Guid? InviterId { get; set; }

        /// <summary>
        /// 是否开启消息免打扰
        /// </summary>
        public bool IsMuted { get; set; } = false;

        /// <summary>
        /// 最后一次阅读消息的时间
        /// </summary>
        public DateTime? LastReadTime { get; set; }

        /// <summary>
        /// 群组
        /// </summary>
        public virtual Group Group { get; set; }

        /// <summary>
        /// 成员用户
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// 邀请人（如果存在）
        /// </summary>
        public virtual User Inviter { get; set; }
    }
}