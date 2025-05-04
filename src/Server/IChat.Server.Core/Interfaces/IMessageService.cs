using IChat.Domain.Entities;
using IChat.Domain.Enums;
using IChat.Protocol.Dtos.Message;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Server.Core.Interfaces
{
    /// <summary>
    /// 消息服务接口，提供消息相关的业务逻辑
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// 发送私聊消息
        /// </summary>
        /// <param name="senderId">发送者ID</param>
        /// <param name="recipientId">接收者ID</param>
        /// <param name="messageType">消息类型</param>
        /// <param name="content">消息内容</param>
        /// <param name="replyToMessageId">回复的消息ID（可选）</param>
        /// <param name="clientMessageId">客户端消息ID（用于确认）</param>
        /// <param name="extendedData">扩展数据（JSON格式）</param>
        /// <returns>已保存的消息</returns>
        Task<Message> SendPrivateMessageAsync(
            Guid senderId, 
            Guid recipientId, 
            MessageType messageType, 
            string content, 
            Guid? replyToMessageId = null, 
            Guid? clientMessageId = null,
            string extendedData = null);

        /// <summary>
        /// 发送群组消息
        /// </summary>
        /// <param name="senderId">发送者ID</param>
        /// <param name="groupId">群组ID</param>
        /// <param name="messageType">消息类型</param>
        /// <param name="content">消息内容</param>
        /// <param name="replyToMessageId">回复的消息ID（可选）</param>
        /// <param name="clientMessageId">客户端消息ID（用于确认）</param>
        /// <param name="extendedData">扩展数据（JSON格式）</param>
        /// <returns>已保存的消息</returns>
        Task<Message> SendGroupMessageAsync(
            Guid senderId, 
            Guid groupId, 
            MessageType messageType, 
            string content, 
            Guid? replyToMessageId = null, 
            Guid? clientMessageId = null,
            string extendedData = null);

        /// <summary>
        /// 获取私聊消息历史
        /// </summary>
        /// <param name="user1Id">用户1 ID</param>
        /// <param name="user2Id">用户2 ID</param>
        /// <param name="pageToken">分页令牌</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="direction">获取方向（向前/向后）</param>
        /// <returns>消息列表和下一页令牌</returns>
        Task<(IEnumerable<MessageDto> Messages, string NextPageToken)> GetPrivateMessageHistoryAsync(
            Guid user1Id, 
            Guid user2Id, 
            string pageToken = null, 
            int pageSize = 20,
            string direction = "Backward");

        /// <summary>
        /// 获取群组消息历史
        /// </summary>
        /// <param name="userId">请求用户ID</param>
        /// <param name="groupId">群组ID</param>
        /// <param name="pageToken">分页令牌</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="direction">获取方向（向前/向后）</param>
        /// <returns>消息列表和下一页令牌</returns>
        Task<(IEnumerable<MessageDto> Messages, string NextPageToken)> GetGroupMessageHistoryAsync(
            Guid userId,
            Guid groupId, 
            string pageToken = null, 
            int pageSize = 20,
            string direction = "Backward");

        /// <summary>
        /// 标记消息为已读
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="messageIds">消息ID列表</param>
        /// <returns>操作是否成功</returns>
        Task<bool> MarkMessagesAsReadAsync(Guid userId, IEnumerable<Guid> messageIds);

        /// <summary>
        /// 标记会话所有消息为已读
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="conversationType">会话类型</param>
        /// <param name="targetId">目标ID（用户ID或群组ID）</param>
        /// <returns>操作是否成功</returns>
        Task<bool> MarkConversationAsReadAsync(Guid userId, ConversationType conversationType, Guid targetId);

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <param name="userId">操作用户ID</param>
        /// <returns>操作是否成功以及结果信息</returns>
        Task<(bool Success, string Message)> RecallMessageAsync(Guid messageId, Guid userId);

        /// <summary>
        /// 获取用户未读消息数
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>未读消息总数</returns>
        Task<int> GetUserUnreadMessageCountAsync(Guid userId);

        /// <summary>
        /// 获取会话未读消息数
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="conversationType">会话类型</param>
        /// <param name="targetId">目标ID（用户ID或群组ID）</param>
        /// <returns>未读消息数</returns>
        Task<int> GetConversationUnreadMessageCountAsync(Guid userId, ConversationType conversationType, Guid targetId);

        /// <summary>
        /// 获取消息详情
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <returns>消息详情</returns>
        Task<MessageDto> GetMessageByIdAsync(Guid messageId);

        /// <summary>
        /// 获取消息已读状态
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <returns>已读回执列表</returns>
        Task<IEnumerable<MessageReadReceiptDto>> GetMessageReadStatusAsync(Guid messageId);

        /// <summary>
        /// 转发消息
        /// </summary>
        /// <param name="messageId">要转发的消息ID</param>
        /// <param name="senderId">发送者ID</param>
        /// <param name="conversationType">目标会话类型</param>
        /// <param name="targetId">目标ID（用户ID或群组ID）</param>
        /// <returns>新创建的消息</returns>
        Task<Message> ForwardMessageAsync(Guid messageId, Guid senderId, ConversationType conversationType, Guid targetId);

        /// <summary>
        /// 获取用户离线消息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="lastSyncTimestamp">上次同步时间戳</param>
        /// <returns>离线消息列表</returns>
        Task<IEnumerable<MessageDto>> GetOfflineMessagesAsync(Guid userId, DateTime lastSyncTimestamp);

        /// <summary>
        /// 删除消息（仅针对自己）
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>操作是否成功</returns>
        Task<bool> DeleteMessageForUserAsync(Guid messageId, Guid userId);
    }
}