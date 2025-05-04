using IChat.Domain.Entities;
using IChat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// Conversation 实体的仓储接口，继承自通用仓储接口，添加特定于会话的操作
    /// </summary>
    public interface IConversationRepository : IRepository<Conversation>
    {
        /// <summary>
        /// 获取用户的所有会话
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>会话列表</returns>
        Task<IEnumerable<Conversation>> GetUserConversationsAsync(Guid userId);
        
        /// <summary>
        /// 获取用户的会话，支持分页和排序
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pageIndex">页码，从 1 开始</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>分页后的会话列表和总数</returns>
        Task<(IEnumerable<Conversation> Items, int TotalCount)> GetUserConversationsPagedAsync(Guid userId, int pageIndex, int pageSize);
        
        /// <summary>
        /// 获取用户与特定目标的会话
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="targetId">目标ID（用户ID或群组ID）</param>
        /// <param name="type">会话类型</param>
        /// <returns>会话实体，不存在则返回 null</returns>
        Task<Conversation> GetConversationAsync(Guid userId, Guid targetId, ConversationType type);
        
        /// <summary>
        /// 获取用户的未读消息会话列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>包含未读消息的会话列表</returns>
        Task<IEnumerable<Conversation>> GetUnreadConversationsAsync(Guid userId);
        
        /// <summary>
        /// 更新会话的最后一条消息
        /// </summary>
        /// <param name="conversationId">会话ID</param>
        /// <param name="messageId">消息ID</param>
        /// <returns>更新后的会话</returns>
        Task<Conversation> UpdateLastMessageAsync(Guid conversationId, Guid messageId);
        
        /// <summary>
        /// 更新会话的未读消息数量
        /// </summary>
        /// <param name="conversationId">会话ID</param>
        /// <param name="unreadCount">未读消息数量</param>
        /// <returns>更新后的会话</returns>
        Task<Conversation> UpdateUnreadCountAsync(Guid conversationId, int unreadCount);
        
        /// <summary>
        /// 将会话标记为已读
        /// </summary>
        /// <param name="conversationId">会话ID</param>
        /// <returns>更新后的会话</returns>
        Task<Conversation> MarkAsReadAsync(Guid conversationId);
        
        /// <summary>
        /// 设置会话置顶状态
        /// </summary>
        /// <param name="conversationId">会话ID</param>
        /// <param name="isPinned">是否置顶</param>
        /// <returns>更新后的会话</returns>
        Task<Conversation> SetPinnedStatusAsync(Guid conversationId, bool isPinned);
        
        /// <summary>
        /// 设置会话静音状态
        /// </summary>
        /// <param name="conversationId">会话ID</param>
        /// <param name="isMuted">是否静音</param>
        /// <returns>更新后的会话</returns>
        Task<Conversation> SetMutedStatusAsync(Guid conversationId, bool isMuted);
    }
}