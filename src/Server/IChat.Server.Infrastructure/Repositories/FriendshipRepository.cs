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
    /// Friendship实体的仓储实现类
    /// </summary>
    public class FriendshipRepository : Repository<Friendship>, IFriendshipRepository
    {
        public FriendshipRepository(IChatDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Friendship>> GetUserFriendshipsAsync(Guid userId)
        {
            return await _dbSet
                .Where(f => (f.InitiatorId == userId || f.RecipientId == userId) && 
                            f.Status == FriendshipStatus.Accepted && 
                            !f.IsDeleted)
                .Include(f => f.Initiator)
                .Include(f => f.Recipient)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUserFriendsAsync(Guid userId)
        {
            var friendshipsAsInitiator = await _dbSet
                .Where(f => f.InitiatorId == userId && 
                            f.Status == FriendshipStatus.Accepted && 
                            !f.IsDeleted)
                .Include(f => f.Recipient)
                .Select(f => f.Recipient)
                .ToListAsync();

            var friendshipsAsRecipient = await _dbSet
                .Where(f => f.RecipientId == userId && 
                            f.Status == FriendshipStatus.Accepted && 
                            !f.IsDeleted)
                .Include(f => f.Initiator)
                .Select(f => f.Initiator)
                .ToListAsync();

            return friendshipsAsInitiator.Concat(friendshipsAsRecipient).ToList();
        }

        public async Task<IEnumerable<Friendship>> GetInitiatedFriendRequestsAsync(Guid userId)
        {
            return await _dbSet
                .Where(f => f.InitiatorId == userId && 
                            f.Status == FriendshipStatus.Pending && 
                            !f.IsDeleted)
                .Include(f => f.Recipient)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Friendship>> GetReceivedFriendRequestsAsync(Guid userId)
        {
            return await _dbSet
                .Where(f => f.RecipientId == userId && 
                            f.Status == FriendshipStatus.Pending && 
                            !f.IsDeleted)
                .Include(f => f.Initiator)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<Friendship> GetFriendshipBetweenUsersAsync(Guid userId1, Guid userId2)
        {
            return await _dbSet
                .Where(f => ((f.InitiatorId == userId1 && f.RecipientId == userId2) || 
                             (f.InitiatorId == userId2 && f.RecipientId == userId1)) && 
                             !f.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AreFriendsAsync(Guid userId1, Guid userId2)
        {
            return await _dbSet
                .AnyAsync(f => ((f.InitiatorId == userId1 && f.RecipientId == userId2) || 
                                (f.InitiatorId == userId2 && f.RecipientId == userId1)) && 
                                f.Status == FriendshipStatus.Accepted && 
                                !f.IsDeleted);
        }

        public async Task<IEnumerable<Friendship>> GetFriendshipsByStatusAsync(Guid userId, FriendshipStatus status)
        {
            return await _dbSet
                .Where(f => (f.InitiatorId == userId || f.RecipientId == userId) && 
                            f.Status == status && 
                            !f.IsDeleted)
                .Include(f => f.Initiator)
                .Include(f => f.Recipient)
                .ToListAsync();
        }

        public async Task<Friendship> UpdateFriendshipStatusAsync(Guid friendshipId, FriendshipStatus status)
        {
            var friendship = await _dbSet.FindAsync(friendshipId);
            
            if (friendship == null)
            {
                throw new ArgumentException($"未找到ID为{friendshipId}的好友关系", nameof(friendshipId));
            }

            friendship.Status = status;
            
            if (status == FriendshipStatus.Accepted || status == FriendshipStatus.Rejected)
            {
                friendship.ResponseTime = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
            return friendship;
        }
    }
}