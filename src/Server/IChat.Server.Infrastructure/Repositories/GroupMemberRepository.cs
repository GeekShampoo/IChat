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
    /// GroupMember实体的仓储实现类
    /// </summary>
    public class GroupMemberRepository : Repository<GroupMember>, IGroupMemberRepository
    {
        public GroupMemberRepository(IChatDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<GroupMember>> GetUserGroupMembershipsAsync(Guid userId)
        {
            return await _dbSet
                .Where(m => m.UserId == userId && !m.IsDeleted)
                .Include(m => m.Group)
                .ToListAsync();
        }

        public async Task<IEnumerable<GroupMember>> GetGroupMembersAsync(Guid groupId)
        {
            return await _dbSet
                .Where(m => m.GroupId == groupId && !m.IsDeleted)
                .Include(m => m.User)
                .ToListAsync();
        }

        public async Task<(IEnumerable<GroupMember> Items, int TotalCount)> GetGroupMembersPagedAsync(Guid groupId, int pageIndex, int pageSize)
        {
            IQueryable<GroupMember> query = _dbSet
                .Where(m => m.GroupId == groupId && !m.IsDeleted)
                .Include(m => m.User);

            int totalCount = await query.CountAsync();

            var pagedItems = await query
                .OrderBy(m => m.Role)
                .ThenBy(m => m.User.Username)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (pagedItems, totalCount);
        }

        public async Task<IEnumerable<GroupMember>> GetMembersByRoleAsync(Guid groupId, GroupMemberRole role)
        {
            return await _dbSet
                .Where(m => m.GroupId == groupId && m.Role == role && !m.IsDeleted)
                .Include(m => m.User)
                .ToListAsync();
        }

        public async Task<GroupMember> GetMembershipAsync(Guid groupId, Guid userId)
        {
            return await _dbSet
                .Where(m => m.GroupId == groupId && m.UserId == userId && !m.IsDeleted)
                .Include(m => m.User)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsMemberAsync(Guid groupId, Guid userId)
        {
            return await _dbSet
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId && !m.IsDeleted);
        }

        public async Task<GroupMember> UpdateMemberRoleAsync(Guid groupId, Guid userId, GroupMemberRole role)
        {
            var member = await _dbSet
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId && !m.IsDeleted);
                
            if (member == null)
            {
                throw new ArgumentException($"未找到群组{groupId}中的成员{userId}", nameof(userId));
            }

            member.Role = role;
            member.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return member;
        }

        public async Task<GroupMember> UpdateMemberNicknameAsync(Guid groupId, Guid userId, string nickname)
        {
            var member = await _dbSet
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId && !m.IsDeleted);
                
            if (member == null)
            {
                throw new ArgumentException($"未找到群组{groupId}中的成员{userId}", nameof(userId));
            }

            member.DisplayName = nickname;
            member.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return member;
        }

        public async Task<IEnumerable<GroupMember>> GetOnlineMembersAsync(Guid groupId)
        {
            return await _dbSet
                .Where(m => m.GroupId == groupId && !m.IsDeleted)
                .Include(m => m.User)
                .Where(m => m.User.Status == UserStatus.Online)
                .ToListAsync();
        }

        public async Task<int> GetMemberCountAsync(Guid groupId)
        {
            return await _dbSet
                .CountAsync(m => m.GroupId == groupId && !m.IsDeleted);
        }
    }
}