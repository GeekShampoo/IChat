using IChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// MessageReadReceipt 实体的仓储接口，继承自通用仓储接口，添加特定于消息已读回执的操作
    /// </summary>
    public interface IMessageReadReceiptRepository : IRepository<MessageReadReceipt>
    {
        /// <summary>
        /// 获取消息的所有已读回执
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <returns>已读回执列表</returns>
        Task<IEnumerable<MessageReadReceipt>> GetMessageReadReceiptsAsync(Guid messageId);
        
        /// <summary>
        /// 获取用户对特定消息的已读回执
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>已读回执，不存在则返回 null</returns>
        Task<MessageReadReceipt> GetUserReadReceiptAsync(Guid messageId, Guid userId);
        
        /// <summary>
        /// 检查用户是否已读某条消息
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>如果已读则返回 true，否则返回 false</returns>
        Task<bool> HasUserReadMessageAsync(Guid messageId, Guid userId);
        
        /// <summary>
        /// 获取用户已读的消息ID列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="messageIds">要检查的消息ID列表</param>
        /// <returns>已读消息ID列表</returns>
        Task<IEnumerable<Guid>> GetUserReadMessagesAsync(Guid userId, IEnumerable<Guid> messageIds);
        
        /// <summary>
        /// 批量添加已读回执
        /// </summary>
        /// <param name="receipts">已读回执列表</param>
        /// <returns>添加成功的回执数量</returns>
        Task<int> AddReadReceiptsAsync(IEnumerable<MessageReadReceipt> receipts);
        
        /// <summary>
        /// 获取消息的已读用户数量
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <returns>已读用户数量</returns>
        Task<int> GetReadCountAsync(Guid messageId);
        
        /// <summary>
        /// 获取群组消息的未读用户列表
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <param name="groupId">群组ID</param>
        /// <returns>未读用户列表</returns>
        Task<IEnumerable<User>> GetUnreadUsersForGroupMessageAsync(Guid messageId, Guid groupId);
    }
}