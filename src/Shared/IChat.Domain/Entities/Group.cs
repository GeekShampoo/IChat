using System;
using System.Collections.Generic;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 群组实体，表示聊天群组
    /// </summary>
    public class Group : BaseEntity
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
        /// 创建者用户ID
        /// </summary>
        public Guid CreatorId { get; set; }

        /// <summary>
        /// 最大成员数量
        /// </summary>
        public int MaxMemberCount { get; set; } = 200;

        /// <summary>
        /// 是否需要验证才能加入
        /// </summary>
        public bool RequiresApproval { get; set; } = true;

        /// <summary>
        /// 是否允许群成员邀请他人
        /// </summary>
        public bool AllowMemberInvitation { get; set; } = false;

        /// <summary>
        /// 是否允许群成员查看历史消息
        /// </summary>
        public bool AllowViewHistoryMessages { get; set; } = true;

        /// <summary>
        /// 创建者（群主）
        /// </summary>
        public virtual User Creator { get; set; }

        /// <summary>
        /// 群组成员
        /// </summary>
        public virtual ICollection<GroupMember> Members { get; set; }

        /// <summary>
        /// 群组消息
        /// </summary>
        public virtual ICollection<Message> Messages { get; set; }

        public Group()
        {
            Members = new List<GroupMember>();
            Messages = new List<Message>();
        }
    }
}