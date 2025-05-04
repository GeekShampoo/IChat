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
    /// User实体的仓储实现类
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IChatDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("用户名不能为空", nameof(username));
            }

            return await _dbSet
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("电子邮箱不能为空", nameof(email));
            }

            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("角色不能为空", nameof(role));
            }

            UserRole userRole;
            if (Enum.TryParse(role, out userRole))
            {
                return await _dbSet
                    .Where(u => u.Role == userRole)
                    .ToListAsync();
            }
            
            // 如果无法解析角色字符串，返回空列表
            return new List<User>();
        }

        public async Task<User> ValidateCredentialsAsync(string username, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("用户名不能为空", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentException("密码哈希不能为空", nameof(passwordHash));
            }

            return await _dbSet
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == passwordHash);
        }

        public async Task<IEnumerable<UserDevice>> GetUserOnlineDevicesAsync(Guid userId)
        {
            return await _dbContext.Set<UserDevice>()
                .Where(d => d.UserId == userId && d.Status == DeviceStatus.Online)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUserFriendsAsync(Guid userId)
        {
            // 通过好友关系表获取所有好友Id (用户可能是发起者或接收者)
            var friendIdsAsInitiator = await _dbContext.Set<Friendship>()
                .Where(f => f.InitiatorId == userId && f.Status == FriendshipStatus.Accepted)
                .Select(f => f.RecipientId)
                .ToListAsync();
                
            var friendIdsAsRecipient = await _dbContext.Set<Friendship>()
                .Where(f => f.RecipientId == userId && f.Status == FriendshipStatus.Accepted)
                .Select(f => f.InitiatorId)
                .ToListAsync();
                
            var allFriendIds = friendIdsAsInitiator.Concat(friendIdsAsRecipient).Distinct();

            // 获取所有好友信息
            return await _dbSet
                .Where(u => allFriendIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetUserGroupsAsync(Guid userId)
        {
            // 获取该用户所在的所有群组ID
            var groupIds = await _dbContext.Set<GroupMember>()
                .Where(m => m.UserId == userId)
                .Select(m => m.GroupId)
                .ToListAsync();

            // 获取所有群组信息
            return await _dbContext.Set<Group>()
                .Where(g => groupIds.Contains(g.Id))
                .ToListAsync();
        }
    }
}