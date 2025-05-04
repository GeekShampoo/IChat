using IChat.Domain.Entities;
using IChat.Domain.Interfaces;
using IChat.Domain.Enums;
using IChat.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IChat.Server.Infrastructure.Repositories
{
    /// <summary>
    /// Message实体的仓储实现类
    /// </summary>
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(IChatDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// 获取私聊消息历史
        /// </summary>
        public async Task<IEnumerable<Message>> GetPrivateMessageHistoryAsync(Guid user1Id, Guid user2Id, Guid? beforeMessageId, int count)
        {
            // 验证参数
            if (user1Id == Guid.Empty)
                throw new ArgumentException("用户1 ID不能为空", nameof(user1Id));
            if (user2Id == Guid.Empty)
                throw new ArgumentException("用户2 ID不能为空", nameof(user2Id));
            if (count <= 0)
                count = 20; // 默认获取20条

            // 构建查询条件
            // 私聊消息条件: (发送者=user1Id AND 接收者=user2Id) OR (发送者=user2Id AND 接收者=user1Id)
            var query = _dbSet
                .Where(m => (m.SenderId == user1Id && m.RecipientId == user2Id) || 
                            (m.SenderId == user2Id && m.RecipientId == user1Id))
                .Where(m => m.GroupId == null); // 确保不是群组消息

            // 如果指定了消息ID基准点，则获取此ID之前的消息
            if (beforeMessageId.HasValue && beforeMessageId.Value != Guid.Empty)
            {
                // 先获取基准消息的时间
                var baseMessage = await _dbSet.FirstOrDefaultAsync(m => m.Id == beforeMessageId.Value);
                if (baseMessage != null)
                {
                    query = query.Where(m => m.SendTime < baseMessage.SendTime);
                }
            }

            // 按时间降序排序并限制数量
            return await query
                .OrderByDescending(m => m.SendTime)
                .Take(count)
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .Include(m => m.ReplyToMessage)
                .Include(m => m.ReadReceipts)
                .ToListAsync();
        }

        /// <summary>
        /// 获取群组消息历史
        /// </summary>
        public async Task<IEnumerable<Message>> GetGroupMessageHistoryAsync(Guid groupId, Guid? beforeMessageId, int count)
        {
            // 验证参数
            if (groupId == Guid.Empty)
                throw new ArgumentException("群组ID不能为空", nameof(groupId));
            if (count <= 0)
                count = 20; // 默认获取20条

            // 构建查询
            var query = _dbSet
                .Where(m => m.GroupId == groupId);

            // 如果指定了消息ID基准点，则获取此ID之前的消息
            if (beforeMessageId.HasValue && beforeMessageId.Value != Guid.Empty)
            {
                // 先获取基准消息的时间
                var baseMessage = await _dbSet.FirstOrDefaultAsync(m => m.Id == beforeMessageId.Value);
                if (baseMessage != null)
                {
                    query = query.Where(m => m.SendTime < baseMessage.SendTime);
                }
            }

            // 按时间降序排序并限制数量
            return await query
                .OrderByDescending(m => m.SendTime)
                .Take(count)
                .Include(m => m.Sender)
                .Include(m => m.Group)
                .Include(m => m.ReplyToMessage)
                .Include(m => m.ReadReceipts)
                .ToListAsync();
        }

        /// <summary>
        /// 获取用户的未读消息数量
        /// </summary>
        public async Task<int> GetUserUnreadMessageCountAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("用户ID不能为空", nameof(userId));

            // 获取私聊未读消息数
            int privateUnreadCount = await _dbSet
                .CountAsync(m => m.RecipientId == userId && 
                                 m.Status != MessageStatus.Read && 
                                 m.Status != MessageStatus.Recalled);

            // 获取群组未读消息数
            // 从用户所在的群组中，查找用户尚未阅读的消息
            // 即存在于群组的消息，但在用户的阅读回执中找不到的消息
            
            // 首先获取用户所在的群组ID列表
            var userGroupIds = await _dbContext.Set<GroupMember>()
                .Where(gm => gm.UserId == userId)
                .Select(gm => gm.GroupId)
                .ToListAsync();

            // 查找这些群组中的消息，但排除用户自己发送的消息和用户已读的消息
            int groupUnreadCount = 0;
            
            if (userGroupIds.Any())
            {
                // 获取用户已读的群组消息ID列表
                var readMessageIds = await _dbContext.Set<MessageReadReceipt>()
                    .Where(r => r.ReadByUserId == userId)
                    .Select(r => r.MessageId)
                    .ToListAsync();

                // 计算群组中未被用户阅读的消息数量
                groupUnreadCount = await _dbSet
                    .CountAsync(m => m.GroupId != null && 
                                   userGroupIds.Contains(m.GroupId.Value) && 
                                   m.SenderId != userId && 
                                   m.Status != MessageStatus.Recalled &&
                                   !readMessageIds.Contains(m.Id));
            }

            return privateUnreadCount + groupUnreadCount;
        }

        /// <summary>
        /// 获取特定会话的未读消息数量
        /// </summary>
        public async Task<int> GetConversationUnreadMessageCountAsync(Guid userId, Guid conversationId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("用户ID不能为空", nameof(userId));
            if (conversationId == Guid.Empty)
                throw new ArgumentException("会话ID不能为空", nameof(conversationId));

            // 首先检查会话类型，是私聊还是群聊
            var conversation = await _dbContext.Set<Conversation>()
                .FirstOrDefaultAsync(c => c.Id == conversationId && c.OwnerId == userId);

            if (conversation == null)
                return 0; // 会话不存在

            if (conversation.Type == ConversationType.Private)
            {
                // 私聊消息：查找接收者是当前用户且发送者是目标用户的未读消息
                return await _dbSet
                    .CountAsync(m => m.RecipientId == userId && 
                                  m.SenderId == conversation.TargetId && 
                                  m.Status != MessageStatus.Read && 
                                  m.Status != MessageStatus.Recalled);
            }
            else if (conversation.Type == ConversationType.Group)
            {
                // 群聊消息：查找群组中非用户自己发送且用户未阅读的消息
                
                // 获取用户已读的群组消息ID列表
                var readMessageIds = await _dbContext.Set<MessageReadReceipt>()
                    .Where(r => r.ReadByUserId == userId)
                    .Select(r => r.MessageId)
                    .ToListAsync();

                // 计算群组中未被用户阅读的消息数量
                return await _dbSet
                    .CountAsync(m => m.GroupId == conversation.TargetId && 
                                   m.SenderId != userId && 
                                   m.Status != MessageStatus.Recalled &&
                                   !readMessageIds.Contains(m.Id));
            }

            return 0;
        }

        /// <summary>
        /// 标记消息为已读
        /// </summary>
        public async Task<bool> MarkMessagesAsReadAsync(IEnumerable<Guid> messageIds, Guid userId)
        {
            if (messageIds == null || !messageIds.Any())
                throw new ArgumentException("消息ID列表不能为空", nameof(messageIds));
            if (userId == Guid.Empty)
                throw new ArgumentException("用户ID不能为空", nameof(userId));

            try
            {
                var messages = await _dbSet
                    .Where(m => messageIds.Contains(m.Id))
                    .ToListAsync();

                if (!messages.Any())
                    return false;

                var now = DateTime.UtcNow;
                var readReceipts = new List<MessageReadReceipt>();

                foreach (var message in messages)
                {
                    // 对于私聊消息，直接更新状态
                    if (message.GroupId == null && message.RecipientId == userId)
                    {
                        message.Status = MessageStatus.Read;
                        message.ReadTime = now;
                        _dbContext.Entry(message).State = EntityState.Modified;
                    }
                    // 对于群聊消息，添加已读回执
                    else if (message.GroupId != null)
                    {
                        // 检查是否已存在已读回执
                        var existingReceipt = await _dbContext.Set<MessageReadReceipt>()
                            .FirstOrDefaultAsync(r => r.MessageId == message.Id && r.ReadByUserId == userId);

                        if (existingReceipt == null)
                        {
                            var receipt = new MessageReadReceipt
                            {
                                Id = Guid.NewGuid(),
                                MessageId = message.Id,
                                ReadByUserId = userId,
                                ReadAt = now
                            };
                            readReceipts.Add(receipt);
                        }
                    }
                }

                // 批量添加已读回执
                if (readReceipts.Any())
                {
                    await _dbContext.Set<MessageReadReceipt>().AddRangeAsync(readReceipts);
                }

                // 保存更改
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 标记会话中的所有消息为已读
        /// </summary>
        public async Task<bool> MarkConversationAsReadAsync(Guid userId, Guid conversationId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("用户ID不能为空", nameof(userId));
            if (conversationId == Guid.Empty)
                throw new ArgumentException("会话ID不能为空", nameof(conversationId));

            try
            {
                // 获取会话信息
                var conversation = await _dbContext.Set<Conversation>()
                    .FirstOrDefaultAsync(c => c.Id == conversationId && c.OwnerId == userId);

                if (conversation == null)
                    return false;

                var now = DateTime.UtcNow;

                if (conversation.Type == ConversationType.Private)
                {
                    // 私聊：标记所有接收者是当前用户的消息为已读
                    var privateMessages = await _dbSet
                        .Where(m => m.RecipientId == userId && 
                                  m.SenderId == conversation.TargetId && 
                                  m.Status != MessageStatus.Read && 
                                  m.Status != MessageStatus.Recalled)
                        .ToListAsync();

                    foreach (var message in privateMessages)
                    {
                        message.Status = MessageStatus.Read;
                        message.ReadTime = now;
                        _dbContext.Entry(message).State = EntityState.Modified;
                    }
                }
                else if (conversation.Type == ConversationType.Group)
                {
                    // 群聊：为所有未读消息创建已读回执
                    
                    // 获取用户已读的群组消息ID列表
                    var readMessageIds = await _dbContext.Set<MessageReadReceipt>()
                        .Where(r => r.ReadByUserId == userId)
                        .Select(r => r.MessageId)
                        .ToListAsync();

                    // 获取群组中未被用户阅读的消息
                    var unreadGroupMessages = await _dbSet
                        .Where(m => m.GroupId == conversation.TargetId && 
                                  m.SenderId != userId && 
                                  m.Status != MessageStatus.Recalled &&
                                  !readMessageIds.Contains(m.Id))
                        .ToListAsync();

                    // 为每条未读消息创建已读回执
                    var readReceipts = unreadGroupMessages.Select(m => new MessageReadReceipt
                    {
                        Id = Guid.NewGuid(),
                        MessageId = m.Id,
                        ReadByUserId = userId,
                        ReadAt = now
                    }).ToList();

                    if (readReceipts.Any())
                    {
                        await _dbContext.Set<MessageReadReceipt>().AddRangeAsync(readReceipts);
                    }
                }

                // 更新会话的最后阅读时间
                conversation.LastReadTime = now;
                _dbContext.Entry(conversation).State = EntityState.Modified;

                // 保存更改
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取消息已读状态
        /// </summary>
        public async Task<IEnumerable<MessageReadReceipt>> GetMessageReadStatusAsync(Guid messageId)
        {
            if (messageId == Guid.Empty)
                throw new ArgumentException("消息ID不能为空", nameof(messageId));

            return await _dbContext.Set<MessageReadReceipt>()
                .Where(r => r.MessageId == messageId)
                .Include(r => r.ReadByUser)
                .ToListAsync();
        }

        /// <summary>
        /// 撤回消息
        /// </summary>
        public async Task<bool> RecallMessageAsync(Guid messageId, Guid userId)
        {
            if (messageId == Guid.Empty)
                throw new ArgumentException("消息ID不能为空", nameof(messageId));
            if (userId == Guid.Empty)
                throw new ArgumentException("用户ID不能为空", nameof(userId));

            // 获取消息
            var message = await _dbSet
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null)
                return false;

            // 验证是否为消息发送者
            if (message.SenderId != userId)
                return false;

            // 检查消息是否可撤回（例如，发送时间在两分钟内）
            var messageAge = DateTime.UtcNow - message.SendTime;
            if (messageAge.TotalMinutes > 2) // 设置2分钟的撤回时间限制
                return false;

            // 更新消息状态为已撤回
            message.Status = MessageStatus.Recalled;
            _dbContext.Entry(message).State = EntityState.Modified;

            // 保存更改
            return await _dbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 获取用户所有会话的最后一条消息
        /// </summary>
        public async Task<IEnumerable<Message>> GetLastMessagesForUserConversationsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("用户ID不能为空", nameof(userId));

            // 获取用户的所有会话
            var conversations = await _dbContext.Set<Conversation>()
                .Where(c => c.OwnerId == userId)
                .ToListAsync();

            var result = new List<Message>();

            foreach (var conversation in conversations)
            {
                Message lastMessage = null;

                if (conversation.Type == ConversationType.Private)
                {
                    // 私聊：获取与特定用户的最后一条消息
                    lastMessage = await _dbSet
                        .Where(m => ((m.SenderId == userId && m.RecipientId == conversation.TargetId) || 
                                    (m.SenderId == conversation.TargetId && m.RecipientId == userId)) &&
                                   m.GroupId == null)
                        .OrderByDescending(m => m.SendTime)
                        .Include(m => m.Sender)
                        .Include(m => m.Recipient)
                        .FirstOrDefaultAsync();
                }
                else if (conversation.Type == ConversationType.Group)
                {
                    // 群聊：获取群组的最后一条消息
                    lastMessage = await _dbSet
                        .Where(m => m.GroupId == conversation.TargetId)
                        .OrderByDescending(m => m.SendTime)
                        .Include(m => m.Sender)
                        .Include(m => m.Group)
                        .FirstOrDefaultAsync();
                }

                if (lastMessage != null)
                {
                    result.Add(lastMessage);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取指定时间之后的消息
        /// </summary>
        public async Task<IEnumerable<Message>> GetMessagesAfterTimestampAsync(Guid userId, DateTime afterTimestamp)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("用户ID不能为空", nameof(userId));

            // 获取私聊消息（发送给用户或用户发送的）
            var privateMessages = await _dbSet
                .Where(m => (m.RecipientId == userId || m.SenderId == userId) && 
                         m.GroupId == null && 
                         m.SendTime > afterTimestamp)
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .Include(m => m.ReplyToMessage)
                .Include(m => m.ReadReceipts)
                .ToListAsync();

            // 获取用户所在群组的ID列表
            var userGroupIds = await _dbContext.Set<GroupMember>()
                .Where(gm => gm.UserId == userId)
                .Select(gm => gm.GroupId)
                .ToListAsync();

            // 获取用户所在群组的消息
            var groupMessages = new List<Message>();
            if (userGroupIds.Any())
            {
                groupMessages = await _dbSet
                    .Where(m => m.GroupId != null && 
                             userGroupIds.Contains(m.GroupId.Value) && 
                             m.SendTime > afterTimestamp)
                    .Include(m => m.Sender)
                    .Include(m => m.Group)
                    .Include(m => m.ReplyToMessage)
                    .Include(m => m.ReadReceipts)
                    .ToListAsync();
            }

            // 合并私聊和群聊消息，并按发送时间排序
            return privateMessages.Concat(groupMessages)
                .OrderByDescending(m => m.SendTime)
                .ToList();
        }

        /// <summary>
        /// 创建消息已读回执
        /// </summary>
        public async Task<MessageReadReceipt> CreateReadReceiptAsync(Guid messageId, Guid userId)
        {
            if (messageId == Guid.Empty)
                throw new ArgumentException("消息ID不能为空", nameof(messageId));
            if (userId == Guid.Empty)
                throw new ArgumentException("用户ID不能为空", nameof(userId));

            // 检查消息是否存在
            var message = await _dbSet.FindAsync(messageId);
            if (message == null)
                return null;

            // 检查是否已存在已读回执
            var existingReceipt = await _dbContext.Set<MessageReadReceipt>()
                .FirstOrDefaultAsync(r => r.MessageId == messageId && r.ReadByUserId == userId);

            if (existingReceipt != null)
                return existingReceipt;

            // 创建新的已读回执
            var receipt = new MessageReadReceipt
            {
                Id = Guid.NewGuid(),
                MessageId = messageId,
                ReadByUserId = userId,
                ReadAt = DateTime.UtcNow
            };

            await _dbContext.Set<MessageReadReceipt>().AddAsync(receipt);
            await _dbContext.SaveChangesAsync();

            return receipt;
        }

        /// <summary>
        /// 获取特定用户的所有未读消息
        /// </summary>
        public async Task<IEnumerable<Message>> GetAllUnreadMessagesForUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("用户ID不能为空", nameof(userId));

            // 获取私聊未读消息
            var privateUnreadMessages = await _dbSet
                .Where(m => m.RecipientId == userId && 
                         m.Status != MessageStatus.Read && 
                         m.Status != MessageStatus.Recalled)
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .Include(m => m.ReplyToMessage)
                .ToListAsync();

            // 获取用户所在的群组ID列表
            var userGroupIds = await _dbContext.Set<GroupMember>()
                .Where(gm => gm.UserId == userId)
                .Select(gm => gm.GroupId)
                .ToListAsync();

            // 群组消息处理
            var groupUnreadMessages = new List<Message>();
            if (userGroupIds.Any())
            {
                // 获取用户已读的群组消息ID列表
                var readMessageIds = await _dbContext.Set<MessageReadReceipt>()
                    .Where(r => r.ReadByUserId == userId)
                    .Select(r => r.MessageId)
                    .ToListAsync();

                // 获取群组中未被用户阅读的消息
                groupUnreadMessages = await _dbSet
                    .Where(m => m.GroupId != null && 
                             userGroupIds.Contains(m.GroupId.Value) && 
                             m.SenderId != userId && 
                             m.Status != MessageStatus.Recalled &&
                             !readMessageIds.Contains(m.Id))
                    .Include(m => m.Sender)
                    .Include(m => m.Group)
                    .Include(m => m.ReplyToMessage)
                    .ToListAsync();
            }

            // 合并私聊和群聊未读消息
            return privateUnreadMessages.Concat(groupUnreadMessages)
                .OrderByDescending(m => m.SendTime)
                .ToList();
        }
    }
}