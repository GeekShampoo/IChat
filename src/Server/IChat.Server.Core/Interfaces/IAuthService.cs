using IChat.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace IChat.Server.Core.Interfaces
{
    /// <summary>
    /// 提供用户身份验证相关功能的服务接口
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码（明文）</param>
        /// <param name="email">电子邮箱</param>
        /// <param name="nickname">昵称</param>
        /// <returns>注册成功返回用户实体，失败返回null</returns>
        Task<User> RegisterAsync(string username, string password, string email, string nickname);

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码（明文）</param>
        /// <returns>登录成功返回JWT令牌，失败返回null</returns>
        Task<string> LoginAsync(string username, string password);

        /// <summary>
        /// 刷新JWT令牌
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        /// <returns>新的JWT令牌，如令牌无效则返回null</returns>
        Task<string> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// 验证令牌有效性
        /// </summary>
        /// <param name="token">JWT令牌</param>
        /// <returns>令牌有效返回true，否则返回false</returns>
        bool ValidateToken(string token);

        /// <summary>
        /// 从JWT令牌中获取用户ID
        /// </summary>
        /// <param name="token">JWT令牌</param>
        /// <returns>用户ID</returns>
        Guid GetUserIdFromToken(string token);

        /// <summary>
        /// 生成密码哈希
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <param name="salt">盐值（如不提供将自动生成）</param>
        /// <returns>密码哈希和使用的盐值</returns>
        (string passwordHash, string salt) HashPassword(string password, string salt = null);

        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <param name="passwordHash">密码哈希</param>
        /// <param name="salt">盐值</param>
        /// <returns>密码正确返回true，否则返回false</returns>
        bool VerifyPassword(string password, string passwordHash, string salt);

        /// <summary>
        /// 创建用户令牌
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="deviceId">设备ID</param>
        /// <param name="ipAddress">IP地址，不能为null</param>
        /// <returns>用户令牌</returns>
        Task<UserToken> CreateUserTokenAsync(Guid userId, string deviceId, string ipAddress);

        /// <summary>
        /// 撤销用户令牌
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        /// <returns>操作成功返回true，否则返回false</returns>
        Task<bool> RevokeTokenAsync(string refreshToken);

        /// <summary>
        /// 撤销用户的所有令牌
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>操作成功返回true，否则返回false</returns>
        Task<bool> RevokeAllTokensAsync(Guid userId);

        /// <summary>
        /// 记录用户登录信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ip">IP地址</param>
        /// <param name="deviceInfo">设备信息</param>
        /// <param name="location">位置信息</param>
        /// <returns>登录日志ID</returns>
        Task<Guid> LogLoginAsync(Guid userId, string ip, string deviceInfo, string location = null);
    }
}