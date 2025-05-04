using IChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// Group 实体的仓储接口，继承自通用仓储接口，添加特定于群组的操作
    /// </summary>
    public interface IGroupRepository : IRepository<Group>
    {
        /// <summary>
        /// 根据群组名称搜索群组
        /// </summary>
        /// <param name="namePattern">群组名称模式</param>
        /// <param name="pageIndex">页码，从 1 开始</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>分页后的群组列表和总数</returns>
        Task<(IEnumerable<Group> Items, int TotalCount)> SearchByNameAsync(string namePattern, int pageIndex, int pageSize);
        
        /// <summary>
        /// 获取用户创建的群组
        /// </summary>
        /// <param name="creatorId">创建者用户ID</param>
        /// <returns>群组列表</returns>
        Task<IEnumerable<Group>> GetGroupsByCreatorAsync(Guid creatorId);
        
        /// <summary>
        /// 获取用户加入的群组
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>群组列表</returns>
        Task<IEnumerable<Group>> GetGroupsByMemberAsync(Guid userId);
        
        /// <summary>
        /// 获取群组成员列表
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <returns>群组成员列表</returns>
        Task<IEnumerable<GroupMember>> GetGroupMembersAsync(Guid groupId);
        
        /// <summary>
        /// 获取群组管理员列表
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <returns>管理员用户列表</returns>
        Task<IEnumerable<User>> GetGroupAdminsAsync(Guid groupId);
        
        /// <summary>
        /// 检查用户是否为群组成员
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="userId">用户ID</param>
        /// <returns>如果是成员则返回 true，否则返回 false</returns>
        Task<bool> IsUserGroupMemberAsync(Guid groupId, Guid userId);
        
        /// <summary>
        /// 获取群组消息
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="pageIndex">页码，从 1 开始</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>分页后的消息列表和总数</returns>
        Task<(IEnumerable<Message> Items, int TotalCount)> GetGroupMessagesAsync(Guid groupId, int pageIndex, int pageSize);
    }
}