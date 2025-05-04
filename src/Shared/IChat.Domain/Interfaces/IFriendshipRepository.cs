using IChat.Domain.Entities;
using IChat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// Friendship 实体的仓储接口，继承自通用仓储接口，添加特定于好友关系的操作
    /// </summary>
    public interface IFriendshipRepository : IRepository<Friendship>
    {
        /// <summary>
        /// 获取用户的好友关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>好友关系列表</returns>
        Task<IEnumerable<Friendship>> GetUserFriendshipsAsync(Guid userId);
        
        /// <summary>
        /// 获取用户的好友列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>好友用户列表</returns>
        Task<IEnumerable<User>> GetUserFriendsAsync(Guid userId);
        
        /// <summary>
        /// 获取用户发起的好友请求
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>好友关系列表</returns>
        Task<IEnumerable<Friendship>> GetInitiatedFriendRequestsAsync(Guid userId);
        
        /// <summary>
        /// 获取用户收到的好友请求
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>好友关系列表</returns>
        Task<IEnumerable<Friendship>> GetReceivedFriendRequestsAsync(Guid userId);
        
        /// <summary>
        /// 获取两个用户之间的好友关系
        /// </summary>
        /// <param name="userId1">用户1 ID</param>
        /// <param name="userId2">用户2 ID</param>
        /// <returns>好友关系，不存在则返回 null</returns>
        Task<Friendship> GetFriendshipBetweenUsersAsync(Guid userId1, Guid userId2);
        
        /// <summary>
        /// 检查两个用户是否为好友
        /// </summary>
        /// <param name="userId1">用户1 ID</param>
        /// <param name="userId2">用户2 ID</param>
        /// <returns>如果是好友则返回 true，否则返回 false</returns>
        Task<bool> AreFriendsAsync(Guid userId1, Guid userId2);
        
        /// <summary>
        /// 获取特定状态的好友关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="status">好友关系状态</param>
        /// <returns>好友关系列表</returns>
        Task<IEnumerable<Friendship>> GetFriendshipsByStatusAsync(Guid userId, FriendshipStatus status);
        
        /// <summary>
        /// 更新好友关系状态
        /// </summary>
        /// <param name="friendshipId">好友关系ID</param>
        /// <param name="status">新状态</param>
        /// <returns>更新后的好友关系</returns>
        Task<Friendship> UpdateFriendshipStatusAsync(Guid friendshipId, FriendshipStatus status);
    }
}