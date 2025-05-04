using IChat.Domain.Entities;
using IChat.Domain.Interfaces;
using IChat.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IChat.Server.Infrastructure.Repositories
{
    /// <summary>
    /// MessageReadReceipt实体的仓储实现类
    /// </summary>
    public class MessageReadReceiptRepository : Repository<MessageReadReceipt>, IMessageReadReceiptRepository
    {
        public MessageReadReceiptRepository(IChatDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<MessageReadReceipt>> GetMessageReadReceiptsAsync(Guid messageId)
        {
            return await _dbSet
                .Where(r => r.MessageId == messageId && !r.IsDeleted)
                .Include(r => r.ReadByUser)
                .ToListAsync();
        }

        public async Task<MessageReadReceipt> GetUserReadReceiptAsync(Guid messageId, Guid userId)
        {
            return await _dbSet
                .Where(r => r.MessageId == messageId && r.ReadByUserId == userId && !r.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasUserReadMessageAsync(Guid messageId, Guid userId)
        {
            return await _dbSet
                .AnyAsync(r => r.MessageId == messageId && r.ReadByUserId == userId && !r.IsDeleted);
        }

        public async Task<IEnumerable<Guid>> GetUserReadMessagesAsync(Guid userId, IEnumerable<Guid> messageIds)
        {
            if (messageIds == null || !messageIds.Any())
            {
                return new List<Guid>();
            }

            return await _dbSet
                .Where(r => r.ReadByUserId == userId && messageIds.Contains(r.MessageId) && !r.IsDeleted)
                .Select(r => r.MessageId)
                .ToListAsync();
        }

        public async Task<int> AddReadReceiptsAsync(IEnumerable<MessageReadReceipt> receipts)
        {
            if (receipts == null || !receipts.Any())
            {
                return 0;
            }

            await _dbSet.AddRangeAsync(receipts);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetReadCountAsync(Guid messageId)
        {
            return await _dbSet
                .CountAsync(r => r.MessageId == messageId && !r.IsDeleted);
        }

        public async Task<IEnumerable<User>> GetUnreadUsersForGroupMessageAsync(Guid messageId, Guid groupId)
        {
            // 获取已读该消息的用户ID列表
            var readUserIds = await _dbSet
                .Where(r => r.MessageId == messageId && !r.IsDeleted)
                .Select(r => r.ReadByUserId)
                .ToListAsync();

            // 获取群组所有成员的用户ID列表
            var groupMemberUserIds = await _dbContext.Set<GroupMember>()
                .Where(m => m.GroupId == groupId && !m.IsDeleted)
                .Select(m => m.UserId)
                .ToListAsync();

            // 未读用户 = 群组成员 - 已读用户
            var unreadUserIds = groupMemberUserIds.Except(readUserIds).ToList();

            // 获取未读用户详细信息
            return await _dbContext.Set<User>()
                .Where(u => unreadUserIds.Contains(u.Id) && !u.IsDeleted)
                .ToListAsync();
        }
    }
}