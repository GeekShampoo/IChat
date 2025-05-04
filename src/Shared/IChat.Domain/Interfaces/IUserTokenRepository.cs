using IChat.Domain.Entities;
using IChat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// UserToken 实体的仓储接口，继承自通用仓储接口，添加特定于用户令牌的操作
    /// </summary>
    public interface IUserTokenRepository : IRepository<UserToken>
    {
        /// <summary>
        /// 获取用户的所有令牌
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>令牌列表</returns>
        Task<IEnumerable<UserToken>> GetUserTokensAsync(Guid userId);
        
        /// <summary>
        /// 获取特定类型的用户令牌
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tokenType">令牌类型</param>
        /// <returns>令牌列表</returns>
        Task<IEnumerable<UserToken>> GetUserTokensByTypeAsync(Guid userId, TokenType tokenType);
        
        /// <summary>
        /// 根据令牌值获取令牌
        /// </summary>
        /// <param name="tokenValue">令牌值</param>
        /// <returns>令牌，不存在则返回 null</returns>
        Task<UserToken> GetByTokenValueAsync(string tokenValue);
        
        /// <summary>
        /// 检查令牌是否有效
        /// </summary>
        /// <param name="tokenValue">令牌值</param>
        /// <param name="tokenType">令牌类型</param>
        /// <returns>如果有效则返回令牌，否则返回 null</returns>
        Task<UserToken> ValidateTokenAsync(string tokenValue, TokenType tokenType);
        
        /// <summary>
        /// 使令牌失效
        /// </summary>
        /// <param name="tokenId">令牌ID</param>
        /// <returns>操作是否成功</returns>
        Task<bool> InvalidateTokenAsync(Guid tokenId);
        
        /// <summary>
        /// 使用户的特定类型令牌全部失效
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tokenType">令牌类型</param>
        /// <returns>失效的令牌数量</returns>
        Task<int> InvalidateUserTokensByTypeAsync(Guid userId, TokenType tokenType);
        
        /// <summary>
        /// 使用户的所有令牌失效
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>失效的令牌数量</returns>
        Task<int> InvalidateAllUserTokensAsync(Guid userId);
        
        /// <summary>
        /// 更新令牌过期时间
        /// </summary>
        /// <param name="tokenId">令牌ID</param>
        /// <param name="expirationTime">新的过期时间</param>
        /// <returns>更新后的令牌</returns>
        Task<UserToken> UpdateTokenExpirationAsync(Guid tokenId, DateTime expirationTime);
        
        /// <summary>
        /// 清理过期令牌
        /// </summary>
        /// <returns>清理的令牌数量</returns>
        Task<int> CleanupExpiredTokensAsync();
    }
}