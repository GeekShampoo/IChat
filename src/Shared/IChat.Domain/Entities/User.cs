using System;
using System.Collections.Generic;
using IChat.Domain.Enums;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 用户实体，表示系统中的用户账号
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// 用户名，用于登录
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 用户昵称，用于显示
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 密码哈希
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// 密码盐
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 个人签名
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public UserStatus Status { get; set; } = UserStatus.Offline;

        /// <summary>
        /// 上次在线时间
        /// </summary>
        public DateTime? LastOnlineTime { get; set; }

        /// <summary>
        /// 用户是否已验证邮箱
        /// </summary>
        public bool IsEmailVerified { get; set; } = false;

        /// <summary>
        /// 用户是否已验证手机
        /// </summary>
        public bool IsPhoneVerified { get; set; } = false;

        /// <summary>
        /// 用户角色（普通用户、管理员等）
        /// </summary>
        public UserRole Role { get; set; } = UserRole.User;

        /// <summary>
        /// 用户的好友关系集合（作为发起方）
        /// </summary>
        public virtual ICollection<Friendship> FriendshipsInitiated { get; set; }

        /// <summary>
        /// 用户的好友关系集合（作为接收方）
        /// </summary>
        public virtual ICollection<Friendship> FriendshipsReceived { get; set; }

        /// <summary>
        /// 用户所属的群组成员关系
        /// </summary>
        public virtual ICollection<GroupMember> GroupMemberships { get; set; }

        /// <summary>
        /// 用户创建的群组
        /// </summary>
        public virtual ICollection<Group> CreatedGroups { get; set; }

        /// <summary>
        /// 用户发送的消息
        /// </summary>
        public virtual ICollection<Message> SentMessages { get; set; }

        /// <summary>
        /// 用户接收的消息（私聊）
        /// </summary>
        public virtual ICollection<Message> ReceivedMessages { get; set; }

        /// <summary>
        /// 用户的消息已读回执
        /// </summary>
        public virtual ICollection<MessageReadReceipt> MessageReadReceipts { get; set; }

        /// <summary>
        /// 用户的设备列表
        /// </summary>
        public virtual ICollection<UserDevice> Devices { get; set; }

        /// <summary>
        /// 用户的令牌列表
        /// </summary>
        public virtual ICollection<UserToken> Tokens { get; set; }

        /// <summary>
        /// 用户的会话列表
        /// </summary>
        public virtual ICollection<Conversation> Conversations { get; set; }

        /// <summary>
        /// 用户的通知列表
        /// </summary>
        public virtual ICollection<Notification> Notifications { get; set; }

        /// <summary>
        /// 用户的设置
        /// </summary>
        public virtual UserSetting Setting { get; set; }

        /// <summary>
        /// 用户的登录记录
        /// </summary>
        public virtual ICollection<UserLoginLog> LoginLogs { get; set; }

        /// <summary>
        /// 用户上传的文件
        /// </summary>
        public virtual ICollection<FileAttachment> UploadedFiles { get; set; }

        public User()
        {
            FriendshipsInitiated = new List<Friendship>();
            FriendshipsReceived = new List<Friendship>();
            GroupMemberships = new List<GroupMember>();
            CreatedGroups = new List<Group>();
            SentMessages = new List<Message>();
            ReceivedMessages = new List<Message>();
            MessageReadReceipts = new List<MessageReadReceipt>();
            Devices = new List<UserDevice>();
            Tokens = new List<UserToken>();
            Conversations = new List<Conversation>();
            Notifications = new List<Notification>();
            LoginLogs = new List<UserLoginLog>();
            UploadedFiles = new List<FileAttachment>();
        }
    }
}