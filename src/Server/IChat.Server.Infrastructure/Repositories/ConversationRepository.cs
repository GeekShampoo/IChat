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
    /// Conversation实体的仓储实现类
    /// </summary>
    public class ConversationRepository : Repository<Conversation>, IConversationRepository
    {
        public ConversationRepository(IChatDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(Guid userId)
        {
            return await _dbSet
                .Where(c => c.OwnerId == userId && !c.IsDeleted)
                .Include(c => c.LastMessage)
                .OrderByDescending(c => c.IsPinned)
                .ThenByDescending(c => c.LastMessage.SendTime)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Conversation> Items, int TotalCount)> GetUserConversationsPagedAsync(Guid userId, int pageIndex, int pageSize)
        {
            IQueryable<Conversation> query = _dbSet
                .Where(c => c.OwnerId == userId && !c.IsDeleted)
                .Include(c => c.LastMessage);

            int totalCount = await query.CountAsync();

            var pagedItems = await query
                .OrderByDescending(c => c.IsPinned)
                .ThenByDescending(c => c.LastMessage.SendTime)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (pagedItems, totalCount);
        }

        public async Task<Conversation> GetConversationAsync(Guid userId, Guid targetId, ConversationType type)
        {
            return await _dbSet
                .Where(c => c.OwnerId == userId && 
                       c.TargetId == targetId && 
                       c.Type == type && 
                       !c.IsDeleted)
                .Include(c => c.LastMessage)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Conversation>> GetUnreadConversationsAsync(Guid userId)
        {
            return await _dbSet
                .Where(c => c.OwnerId == userId && 
                       c.UnreadCount > 0 && 
                       !c.IsDeleted)
                .Include(c => c.LastMessage)
                .OrderByDescending(c => c.LastMessage.SendTime)
                .ToListAsync();
        }

        public async Task<Conversation> UpdateLastMessageAsync(Guid conversationId, Guid messageId)
        {
            var conversation = await _dbSet.FindAsync(conversationId);
            
            if (conversation == null)
            {
                throw new ArgumentException($"未找到ID为{conversationId}的会话", nameof(conversationId));
            }

            conversation.LastMessageId = messageId;
            conversation.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            return conversation;
        }

        public async Task<Conversation> UpdateUnreadCountAsync(Guid conversationId, int unreadCount)
        {
            var conversation = await _dbSet.FindAsync(conversationId);
            
            if (conversation == null)
            {
                throw new ArgumentException($"未找到ID为{conversationId}的会话", nameof(conversationId));
            }

            conversation.UnreadCount = unreadCount;
            conversation.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            return conversation;
        }

        public async Task<Conversation> MarkAsReadAsync(Guid conversationId)
        {
            var conversation = await _dbSet.FindAsync(conversationId);
            
            if (conversation == null)
            {
                throw new ArgumentException($"未找到ID为{conversationId}的会话", nameof(conversationId));
            }

            conversation.UnreadCount = 0;
            conversation.LastReadTime = DateTime.UtcNow;
            conversation.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            return conversation;
        }

        public async Task<Conversation> SetPinnedStatusAsync(Guid conversationId, bool isPinned)
        {
            var conversation = await _dbSet.FindAsync(conversationId);
            
            if (conversation == null)
            {
                throw new ArgumentException($"未找到ID为{conversationId}的会话", nameof(conversationId));
            }

            conversation.IsPinned = isPinned;
            conversation.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            return conversation;
        }

        public async Task<Conversation> SetMutedStatusAsync(Guid conversationId, bool isMuted)
        {
            var conversation = await _dbSet.FindAsync(conversationId);
            
            if (conversation == null)
            {
                throw new ArgumentException($"未找到ID为{conversationId}的会话", nameof(conversationId));
            }

            conversation.IsMuted = isMuted;
            conversation.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            return conversation;
        }
    }
}