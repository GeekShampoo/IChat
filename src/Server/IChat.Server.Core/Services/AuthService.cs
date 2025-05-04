using BCrypt.Net;
using IChat.Domain.Entities;
using IChat.Domain.Enums;
using IChat.Domain.Interfaces;
using IChat.Server.Core.Interfaces;
using IChat.Server.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IChat.Server.Core.Services
{
    /// <summary>
    /// 身份验证服务实现
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtOptions _jwtOptions;

        public AuthService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IOptions<JwtOptions> jwtOptions)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _jwtOptions = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        }

        /// <inheritdoc/>
        public async Task<User> RegisterAsync(string username, string password, string email, string nickname)
        {
            // 验证用户名是否已存在
            if (await _userRepository.ExistsAsync(u => u.Username == username))
            {
                throw new InvalidOperationException($"用户名 '{username}' 已存在");
            }

            // 验证邮箱是否已存在
            if (await _userRepository.ExistsAsync(u => u.Email == email))
            {
                throw new InvalidOperationException($"邮箱 '{email}' 已被注册");
            }

            // 生成密码哈希和盐值
            var (passwordHash, salt) = HashPassword(password);

            // 创建新用户
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                Nickname = nickname,
                PasswordHash = passwordHash,
                PasswordSalt = salt,
                Role = UserRole.User,
                Status = UserStatus.Online,
                AvatarUrl = "/assets/default-avatar.png", // 默认头像
                PhoneNumber = "", // 避免 NULL 值
                Signature = "", // 避免 NULL 值
                CreatedAt = DateTime.UtcNow
            };

            // 创建用户设置
            var userSetting = new UserSetting
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ThemeMode = 0, // 跟随系统
                ThemeColor = "#1890ff",
                EnableNotification = true,
                EnableSoundNotification = true,
                ShowMessagePreview = true,
                NotificationSound = "default",
                Language = "zh-CN",
                AutoLogin = true,
                CreatedAt = DateTime.UtcNow
            };

            // 使用新的事务执行方法，支持自动重试
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // 保存用户
                await _userRepository.AddAsync(user);

                // 保存用户设置
                var settingRepository = _unitOfWork.Repository<UserSetting>();
                await settingRepository.AddAsync(userSetting);

                // 保存更改
                await _unitOfWork.SaveChangesAsync();

                return user;
            });
        }

        /// <inheritdoc/>
        public async Task<string> LoginAsync(string username, string password)
        {
            // 查找用户
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return null; // 用户不存在
            }

            // 验证密码
            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                return null; // 密码错误
            }

            // 生成JWT令牌
            var token = GenerateJwtToken(user);

            // 更新用户状态为在线
            user.Status = UserStatus.Online;
            user.LastOnlineTime = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            return token;
        }

        /// <inheritdoc/>
        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            // 查找刷新令牌
            var tokenRepository = _unitOfWork.Repository<UserToken>();
            var userToken = await tokenRepository.GetSingleAsync(t => 
                t.RefreshToken == refreshToken && 
                t.TokenType == TokenType.Refresh &&
                t.RefreshTokenExpiresAt > DateTime.UtcNow);

            if (userToken == null)
            {
                return null; // 刷新令牌无效或已过期
            }

            // 查找用户
            var user = await _userRepository.GetByIdAsync(userToken.UserId);
            if (user == null)
            {
                return null; // 用户不存在
            }

            // 生成新的JWT令牌
            var token = GenerateJwtToken(user);

            // 更新刷新令牌的过期时间
            userToken.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);
            await tokenRepository.UpdateAsync(userToken);

            return token;
        }

        /// <inheritdoc/>
        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtOptions.Audience,
                    ClockSkew = TimeSpan.Zero // 不允许任何时钟偏差
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public Guid GetUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token cannot be null or empty", nameof(token));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtOptions.Audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    throw new SecurityTokenException("Invalid token: missing user id claim");
                }

                if (!Guid.TryParse(userIdClaim, out Guid userId))
                {
                    throw new SecurityTokenException("Invalid token: user id claim is not a valid GUID");
                }

                return userId;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Invalid token", ex);
            }
        }

        /// <inheritdoc/>
        public (string passwordHash, string salt) HashPassword(string password, string salt = null)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(password));
            }

            // 如果未提供盐值，则生成新的盐值
            if (string.IsNullOrEmpty(salt))
            {
                salt = GenerateSalt();
            }

            // 使用BCrypt生成密码哈希
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password + salt);

            return (passwordHash, salt);
        }

        /// <inheritdoc/>
        public bool VerifyPassword(string password, string passwordHash, string salt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordHash) || string.IsNullOrEmpty(salt))
            {
                return false;
            }

            try
            {
                // 使用BCrypt验证密码
                return BCrypt.Net.BCrypt.Verify(password + salt, passwordHash);
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<UserToken> CreateUserTokenAsync(Guid userId, string deviceId, string ipAddress)
        {
            // 生成刷新令牌
            var refreshToken = GenerateRefreshToken();
            var expiryDate = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);
            
            // 获取用户信息并生成访问令牌
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"用户不存在: {userId}");
            }
            var accessToken = GenerateJwtToken(user);
            var accessTokenExpiryDate = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

            // 修改这里使用 Guid.TryParse 来安全处理 deviceId
            Guid? parsedDeviceId = null;
            if (!string.IsNullOrEmpty(deviceId) && Guid.TryParse(deviceId, out Guid result))
            {
                parsedDeviceId = result;
            }

            var token = new UserToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenType = TokenType.Refresh,
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessTokenExpiryDate,
                RefreshToken = refreshToken,
                DeviceId = parsedDeviceId,
                IpAddress = ipAddress ?? "Unknown", // 设置IP地址，确保不为null
                CreatedAt = DateTime.UtcNow,
                RefreshTokenExpiresAt = expiryDate
            };

            var tokenRepository = _unitOfWork.Repository<UserToken>();
            await tokenRepository.AddAsync(token);

            return token;
        }

        /// <inheritdoc/>
        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }

            var tokenRepository = _unitOfWork.Repository<UserToken>();
            var token = await tokenRepository.GetSingleAsync(t => t.RefreshToken == refreshToken);

            if (token == null)
            {
                return false;
            }

            // 撤销令牌
            return await tokenRepository.DeleteAsync(token);
        }

        /// <inheritdoc/>
        public async Task<bool> RevokeAllTokensAsync(Guid userId)
        {
            var tokenRepository = _unitOfWork.Repository<UserToken>();
            var tokens = await tokenRepository.FindAsync(t => t.UserId == userId);

            if (!tokens.Any())
            {
                return true; // 没有令牌需要撤销
            }

            // 撤销所有令牌
            return await tokenRepository.DeleteRangeAsync(tokens);
        }

        /// <inheritdoc/>
        public async Task<Guid> LogLoginAsync(Guid userId, string ip, string deviceInfo, string location = null)
        {
            var logRepository = _unitOfWork.Repository<UserLoginLog>();
            
            var log = new UserLoginLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LoginTime = DateTime.UtcNow,
                IpAddress = ip ?? "Unknown",
                UserAgent = deviceInfo ?? "Unknown",
                Location = location ?? "Unknown", // 为 Location 提供默认值，避免 NULL
                IsSuccessful = true,
                FailureReason = "", // 为成功登录设置空字符串而非 NULL
                CreatedAt = DateTime.UtcNow
            };

            await logRepository.AddAsync(log);
            return log.Id;
        }

        #region 私有方法

        /// <summary>
        /// 生成JWT令牌
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 生成随机盐值
        /// </summary>
        private string GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// 生成刷新令牌
        /// </summary>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        #endregion
    }
}