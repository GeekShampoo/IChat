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
    /// UserToken实体的仓储实现类
    /// </summary>
    public class UserTokenRepository : Repository<UserToken>, IUserTokenRepository
    {
        public UserTokenRepository(IChatDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<UserToken>> GetUserTokensAsync(Guid userId)
        {
            return await _dbSet
                .Where(t => t.UserId == userId && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserToken>> GetUserTokensByTypeAsync(Guid userId, TokenType tokenType)
        {
            return await _dbSet
                .Where(t => t.UserId == userId && t.TokenType == tokenType && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<UserToken> GetByTokenValueAsync(string tokenValue)
        {
            if (string.IsNullOrWhiteSpace(tokenValue))
            {
                throw new ArgumentException("令牌值不能为空", nameof(tokenValue));
            }

            return await _dbSet
                .FirstOrDefaultAsync(t => t.AccessToken == tokenValue && !t.IsDeleted);
        }

        public async Task<UserToken> ValidateTokenAsync(string tokenValue, TokenType tokenType)
        {
            if (string.IsNullOrWhiteSpace(tokenValue))
            {
                throw new ArgumentException("令牌值不能为空", nameof(tokenValue));
            }

            var now = DateTime.UtcNow;
            
            return await _dbSet
                .FirstOrDefaultAsync(t => t.AccessToken == tokenValue && 
                                    t.TokenType == tokenType && 
                                    t.AccessTokenExpiresAt > now && 
                                    !t.IsRevoked && 
                                    !t.IsDeleted);
        }

        public async Task<bool> InvalidateTokenAsync(Guid tokenId)
        {
            var token = await _dbSet.FindAsync(tokenId);
            
            if (token == null)
            {
                throw new ArgumentException($"未找到ID为{tokenId}的令牌", nameof(tokenId));
            }

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.UpdatedAt = DateTime.UtcNow;
            
            int result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<int> InvalidateUserTokensByTypeAsync(Guid userId, TokenType tokenType)
        {
            var tokens = await _dbSet
                .Where(t => t.UserId == userId && 
                       t.TokenType == tokenType && 
                       !t.IsRevoked && 
                       !t.IsDeleted)
                .ToListAsync();
                
            if (!tokens.Any())
            {
                return 0;
            }
            
            var now = DateTime.UtcNow;
            
            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = now;
                token.UpdatedAt = now;
            }
            
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> InvalidateAllUserTokensAsync(Guid userId)
        {
            var tokens = await _dbSet
                .Where(t => t.UserId == userId && 
                       !t.IsRevoked && 
                       !t.IsDeleted)
                .ToListAsync();
                
            if (!tokens.Any())
            {
                return 0;
            }
            
            var now = DateTime.UtcNow;
            
            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = now;
                token.UpdatedAt = now;
            }
            
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<UserToken> UpdateTokenExpirationAsync(Guid tokenId, DateTime expirationTime)
        {
            var token = await _dbSet.FindAsync(tokenId);
            
            if (token == null)
            {
                throw new ArgumentException($"未找到ID为{tokenId}的令牌", nameof(tokenId));
            }

            token.AccessTokenExpiresAt = expirationTime;
            token.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            return token;
        }

        public async Task<int> CleanupExpiredTokensAsync()
        {
            var now = DateTime.UtcNow;
            
            var expiredTokens = await _dbSet
                .Where(t => t.AccessTokenExpiresAt < now && !t.IsDeleted)
                .ToListAsync();
                
            if (!expiredTokens.Any())
            {
                return 0;
            }
            
            // 软删除过期令牌
            foreach (var token in expiredTokens)
            {
                token.IsDeleted = true;
                token.UpdatedAt = now;
            }
            
            return await _dbContext.SaveChangesAsync();
        }
    }
}