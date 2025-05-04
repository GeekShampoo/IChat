using IChat.Domain.Entities;
using IChat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// GroupMember 实体的仓储接口，继承自通用仓储接口，添加特定于群组成员的操作
    /// </summary>
    public interface IGroupMemberRepository : IRepository<GroupMember>
    {
        /// <summary>
        /// 获取用户加入的所有群组成员关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>群组成员关系列表</returns>
        Task<IEnumerable<GroupMember>> GetUserGroupMembershipsAsync(Guid userId);
        
        /// <summary>
        /// 获取群组的所有成员
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <returns>群组成员关系列表</returns>
        Task<IEnumerable<GroupMember>> GetGroupMembersAsync(Guid groupId);
        
        /// <summary>
        /// 获取群组的所有成员，支持分页
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="pageIndex">页码，从 1 开始</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>分页后的群组成员关系列表和总数</returns>
        Task<(IEnumerable<GroupMember> Items, int TotalCount)> GetGroupMembersPagedAsync(Guid groupId, int pageIndex, int pageSize);
        
        /// <summary>
        /// 获取特定角色的群组成员
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="role">成员角色</param>
        /// <returns>成员列表</returns>
        Task<IEnumerable<GroupMember>> GetMembersByRoleAsync(Guid groupId, GroupMemberRole role);
        
        /// <summary>
        /// 获取用户在群组中的成员关系
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>群组成员关系，不存在则返回 null</returns>
        Task<GroupMember> GetMembershipAsync(Guid groupId, Guid userId);
        
        /// <summary>
        /// 检查用户是否为群组成员
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>如果是成员则返回 true，否则返回 false</returns>
        Task<bool> IsMemberAsync(Guid groupId, Guid userId);
        
        /// <summary>
        /// 更新成员角色
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="role">新角色</param>
        /// <returns>更新后的成员关系</returns>
        Task<GroupMember> UpdateMemberRoleAsync(Guid groupId, Guid userId, GroupMemberRole role);
        
        /// <summary>
        /// 更新成员昵称
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="nickname">新昵称</param>
        /// <returns>更新后的成员关系</returns>
        Task<GroupMember> UpdateMemberNicknameAsync(Guid groupId, Guid userId, string nickname);
        
        /// <summary>
        /// 获取群组在线成员
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <returns>在线成员列表</returns>
        Task<IEnumerable<GroupMember>> GetOnlineMembersAsync(Guid groupId);
        
        /// <summary>
        /// 获取群组成员数量
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <returns>成员数量</returns>
        Task<int> GetMemberCountAsync(Guid groupId);
    }
}