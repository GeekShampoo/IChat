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
    /// Group实体的仓储实现类
    /// </summary>
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(IChatDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(IEnumerable<Group> Items, int TotalCount)> SearchByNameAsync(string namePattern, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(namePattern))
            {
                throw new ArgumentException("搜索名称不能为空", nameof(namePattern));
            }

            IQueryable<Group> query = _dbSet
                .Where(g => g.Name.Contains(namePattern) && !g.IsDeleted);

            int totalCount = await query.CountAsync();

            var pagedGroups = await query
                .OrderBy(g => g.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (pagedGroups, totalCount);
        }

        public async Task<IEnumerable<Group>> GetGroupsByCreatorAsync(Guid creatorId)
        {
            return await _dbSet
                .Where(g => g.CreatorId == creatorId && !g.IsDeleted)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetGroupsByMemberAsync(Guid userId)
        {
            var groupIds = await _dbContext.Set<GroupMember>()
                .Where(m => m.UserId == userId && !m.IsDeleted)
                .Select(m => m.GroupId)
                .ToListAsync();

            return await _dbSet
                .Where(g => groupIds.Contains(g.Id) && !g.IsDeleted)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<GroupMember>> GetGroupMembersAsync(Guid groupId)
        {
            return await _dbContext.Set<GroupMember>()
                .Where(m => m.GroupId == groupId && !m.IsDeleted)
                .Include(m => m.User)
                .OrderBy(m => m.Role)
                .ThenBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetGroupAdminsAsync(Guid groupId)
        {
            return await _dbContext.Set<GroupMember>()
                .Where(m => m.GroupId == groupId && 
                      (m.Role == GroupMemberRole.Admin || m.Role == GroupMemberRole.Owner) && 
                      !m.IsDeleted)
                .Include(m => m.User)
                .Select(m => m.User)
                .ToListAsync();
        }

        public async Task<bool> IsUserGroupMemberAsync(Guid groupId, Guid userId)
        {
            return await _dbContext.Set<GroupMember>()
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId && !m.IsDeleted);
        }

        public async Task<(IEnumerable<Message> Items, int TotalCount)> GetGroupMessagesAsync(Guid groupId, int pageIndex, int pageSize)
        {
            IQueryable<Message> query = _dbContext.Set<Message>()
                .Where(m => m.GroupId == groupId && !m.IsDeleted);

            int totalCount = await query.CountAsync();

            var pagedMessages = await query
                .Include(m => m.Sender)
                .OrderByDescending(m => m.SendTime)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (pagedMessages, totalCount);
        }
    }
}