using IChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// Message 实体的仓储接口，继承自通用仓储接口，添加特定于消息的操作
    /// </summary>
    public interface IMessageRepository : IRepository<Message>
    {
        /// <summary>
        /// 获取私聊消息历史
        /// </summary>
        /// <param name="user1Id">用户1 ID</param>
        /// <param name="user2Id">用户2 ID</param>
        /// <param name="beforeMessageId">消息ID基准点（获取此ID之前的消息，为空则获取最新消息）</param>
        /// <param name="count">获取消息数量</param>
        /// <returns>消息列表，按时间降序排列</returns>
        Task<IEnumerable<Message>> GetPrivateMessageHistoryAsync(Guid user1Id, Guid user2Id, Guid? beforeMessageId, int count);

        /// <summary>
        /// 获取群组消息历史
        /// </summary>
        /// <param name="groupId">群组 ID</param>
        /// <param name="beforeMessageId">消息ID基准点（获取此ID之前的消息，为空则获取最新消息）</param>
        /// <param name="count">获取消息数量</param>
        /// <returns>消息列表，按时间降序排列</returns>
        Task<IEnumerable<Message>> GetGroupMessageHistoryAsync(Guid groupId, Guid? beforeMessageId, int count);

        /// <summary>
        /// 获取用户的未读消息数量
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>未读消息数量</returns>
        Task<int> GetUserUnreadMessageCountAsync(Guid userId);

        /// <summary>
        /// 获取特定会话的未读消息数量
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <param name="conversationId">会话 ID</param>
        /// <returns>未读消息数量</returns>
        Task<int> GetConversationUnreadMessageCountAsync(Guid userId, Guid conversationId);

        /// <summary>
        /// 标记消息为已读
        /// </summary>
        /// <param name="messageIds">消息 ID 列表</param>
        /// <param name="userId">用户 ID</param>
        /// <returns>操作是否成功</returns>
        Task<bool> MarkMessagesAsReadAsync(IEnumerable<Guid> messageIds, Guid userId);

        /// <summary>
        /// 标记会话中的所有消息为已读
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <param name="conversationId">会话 ID</param>
        /// <returns>操作是否成功</returns>
        Task<bool> MarkConversationAsReadAsync(Guid userId, Guid conversationId);

        /// <summary>
        /// 获取消息已读状态
        /// </summary>
        /// <param name="messageId">消息 ID</param>
        /// <returns>已读回执列表</returns>
        Task<IEnumerable<MessageReadReceipt>> GetMessageReadStatusAsync(Guid messageId);

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="messageId">消息 ID</param>
        /// <param name="userId">操作用户 ID，用于验证权限</param>
        /// <returns>操作是否成功</returns>
        Task<bool> RecallMessageAsync(Guid messageId, Guid userId);

        /// <summary>
        /// 获取用户所有会话的最后一条消息
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>最后一条消息列表</returns>
        Task<IEnumerable<Message>> GetLastMessagesForUserConversationsAsync(Guid userId);

        /// <summary>
        /// 获取指定时间之后的消息
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <param name="afterTimestamp">时间戳</param>
        /// <returns>消息列表</returns>
        Task<IEnumerable<Message>> GetMessagesAfterTimestampAsync(Guid userId, DateTime afterTimestamp);

        /// <summary>
        /// 创建消息已读回执
        /// </summary>
        /// <param name="messageId">消息 ID</param>
        /// <param name="userId">用户 ID</param>
        /// <returns>已读回执</returns>
        Task<MessageReadReceipt> CreateReadReceiptAsync(Guid messageId, Guid userId);

        /// <summary>
        /// 获取特定用户的所有未读消息
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>未读消息列表</returns>
        Task<IEnumerable<Message>> GetAllUnreadMessagesForUserAsync(Guid userId);
    }
}