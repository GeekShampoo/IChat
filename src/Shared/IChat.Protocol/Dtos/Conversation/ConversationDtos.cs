using System;
using System.Collections.Generic;
using IChat.Protocol.Contracts;
using IChat.Protocol.Dtos.Message;

namespace IChat.Protocol.Dtos.Conversation
{
    /// <summary>
    /// 会话DTO
    /// </summary>
    public class ConversationDto
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 会话类型（Private/Group/System）
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 目标ID（用户ID或群组ID）
        /// </summary>
        public Guid TargetId { get; set; }

        /// <summary>
        /// 目标名称（用户名或群组名）
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// 目标头像URL
        /// </summary>
        public string TargetAvatar { get; set; }

        /// <summary>
        /// 未读消息数量
        /// </summary>
        public int UnreadCount { get; set; }

        /// <summary>
        /// 最后一条消息
        /// </summary>
        public MessageDto LastMessage { get; set; }

        /// <summary>
        /// 最后活跃时间
        /// </summary>
        public DateTime LastActiveTime { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsPinned { get; set; }

        /// <summary>
        /// 是否静音
        /// </summary>
        public bool IsMuted { get; set; }

        /// <summary>
        /// 草稿（如有）
        /// </summary>
        public string Draft { get; set; }

        /// <summary>
        /// 在线状态（私聊时有效）
        /// </summary>
        public string OnlineStatus { get; set; }
    }

    /// <summary>
    /// 获取会话列表请求
    /// </summary>
    public class GetConversationsRequest : BaseRequest
    {
        /// <summary>
        /// 分页令牌
        /// </summary>
        public string PageToken { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 会话类型筛选
        /// </summary>
        public List<string> Types { get; set; }
    }

    /// <summary>
    /// 更新会话请求
    /// </summary>
    public class UpdateConversationRequest : BaseRequest
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        public Guid ConversationId { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool? IsPinned { get; set; }

        /// <summary>
        /// 是否静音
        /// </summary>
        public bool? IsMuted { get; set; }

        /// <summary>
        /// 草稿内容
        /// </summary>
        public string Draft { get; set; }
    }

    /// <summary>
    /// 删除会话请求
    /// </summary>
    public class DeleteConversationRequest : BaseRequest
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        public Guid ConversationId { get; set; }

        /// <summary>
        /// 是否同时删除消息历史
        /// </summary>
        public bool DeleteMessages { get; set; } = false;
    }

    /// <summary>
    /// 标记会话已读请求
    /// </summary>
    public class MarkConversationReadRequest : BaseRequest
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        public Guid ConversationId { get; set; }
    }

    /// <summary>
    /// 获取会话列表响应
    /// </summary>
    public class GetConversationsResponse : BaseResponse<GetConversationsResultDto>
    {
    }

    /// <summary>
    /// 获取会话列表结果DTO
    /// </summary>
    public class GetConversationsResultDto
    {
        /// <summary>
        /// 会话列表
        /// </summary>
        public List<ConversationDto> Conversations { get; set; } = new List<ConversationDto>();

        /// <summary>
        /// 下一页分页令牌
        /// </summary>
        public string NextPageToken { get; set; }

        /// <summary>
        /// 是否有更多数据
        /// </summary>
        public bool HasMore { get; set; }

        /// <summary>
        /// 总会话数量
        /// </summary>
        public int TotalCount { get; set; }
    }
}