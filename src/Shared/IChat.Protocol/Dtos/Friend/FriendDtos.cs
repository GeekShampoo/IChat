using System;
using System.Collections.Generic;
using IChat.Protocol.Contracts;

namespace IChat.Protocol.Dtos.Friend
{
    /// <summary>
    /// 好友信息DTO
    /// </summary>
    public class FriendDto
    {
        /// <summary>
        /// 好友关系ID
        /// </summary>
        public Guid FriendshipId { get; set; }

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
        /// 备注名
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 个人签名
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 好友分组
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 在线状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 上次在线时间
        /// </summary>
        public DateTime? LastOnlineTime { get; set; }

        /// <summary>
        /// 好友关系建立时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 好友分组DTO
    /// </summary>
    public class FriendGroupDto
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 好友列表
        /// </summary>
        public List<FriendDto> Friends { get; set; } = new List<FriendDto>();
    }

    /// <summary>
    /// 好友请求DTO
    /// </summary>
    public class FriendRequestDto
    {
        /// <summary>
        /// 请求ID
        /// </summary>
        public Guid Id { get; set; }

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
        /// 接收人ID
        /// </summary>
        public Guid RecipientId { get; set; }

        /// <summary>
        /// 接收人用户名
        /// </summary>
        public string RecipientUsername { get; set; }

        /// <summary>
        /// 接收人昵称
        /// </summary>
        public string RecipientNickname { get; set; }

        /// <summary>
        /// 请求消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 请求状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 响应时间
        /// </summary>
        public DateTime? ResponseTime { get; set; }
    }

    /// <summary>
    /// 添加好友请求
    /// </summary>
    public class AddFriendRequest : BaseRequest
    {
        /// <summary>
        /// 要添加的用户ID、用户名或邮箱
        /// </summary>
        public string UserIdentifier { get; set; }

        /// <summary>
        /// 验证消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 好友分组
        /// </summary>
        public string Group { get; set; } = "我的好友";
    }

    /// <summary>
    /// 好友请求响应
    /// </summary>
    public class FriendRequestResponse : BaseRequest
    {
        /// <summary>
        /// 好友请求ID
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// 是否接受
        /// </summary>
        public bool Accept { get; set; }

        /// <summary>
        /// 好友分组
        /// </summary>
        public string Group { get; set; } = "我的好友";

        /// <summary>
        /// 备注名
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 更新好友信息请求
    /// </summary>
    public class UpdateFriendRequest : BaseRequest
    {
        /// <summary>
        /// 好友ID
        /// </summary>
        public Guid FriendId { get; set; }

        /// <summary>
        /// 好友备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 好友分组
        /// </summary>
        public string Group { get; set; }
    }

    /// <summary>
    /// 删除好友请求
    /// </summary>
    public class DeleteFriendRequest : BaseRequest
    {
        /// <summary>
        /// 好友ID
        /// </summary>
        public Guid FriendId { get; set; }
    }
}