using IChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// User 实体的仓储接口，继承自通用仓储接口，添加特定于用户的操作
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户实体，不存在则返回 null</returns>
        Task<User> GetByUsernameAsync(string username);
        
        /// <summary>
        /// 根据电子邮箱获取用户
        /// </summary>
        /// <param name="email">电子邮箱</param>
        /// <returns>用户实体，不存在则返回 null</returns>
        Task<User> GetByEmailAsync(string email);
        
        /// <summary>
        /// 获取特定角色的用户列表
        /// </summary>
        /// <param name="role">用户角色</param>
        /// <returns>用户列表</returns>
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        
        /// <summary>
        /// 验证用户凭据
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="passwordHash">密码哈希</param>
        /// <returns>如果凭据有效则返回用户，否则返回 null</returns>
        Task<User> ValidateCredentialsAsync(string username, string passwordHash);
        
        /// <summary>
        /// 获取用户的所有在线设备
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>设备列表</returns>
        Task<IEnumerable<UserDevice>> GetUserOnlineDevicesAsync(Guid userId);
        
        /// <summary>
        /// 获取用户的好友列表
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>好友用户列表</returns>
        Task<IEnumerable<User>> GetUserFriendsAsync(Guid userId);
        
        /// <summary>
        /// 获取用户所属的群组列表
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <returns>群组列表</returns>
        Task<IEnumerable<Group>> GetUserGroupsAsync(Guid userId);
    }
}