using IChat.Domain.Entities;
using IChat.Domain.Enums;
using IChat.Domain.Interfaces;
using IChat.Protocol.Contracts;
using IChat.Protocol.Dtos.Auth;
using IChat.Protocol.Dtos.User;
using IChat.Server.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace IChat.Server.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService, 
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(RegisterResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegisterResponse 
                { 
                    Success = false, 
                    ErrorMessage = "请求参数无效",
                    Data = null
                });
            }

            try
            {
                var user = await _authService.RegisterAsync(
                    request.Username,
                    request.Password,
                    request.Email,
                    request.Nickname);

                var result = new RegisterResultDto
                {
                    UserId = user.Id,
                    RequiresEmailVerification = !user.IsEmailVerified,
                    CreatedAt = user.CreatedAt
                };

                return Ok(new RegisterResponse
                {
                    Success = true,
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "用户注册失败: {Message}", ex.Message);
                return BadRequest(new RegisterResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户注册过程中发生错误");
                return StatusCode((int)HttpStatusCode.InternalServerError, new RegisterResponse
                {
                    Success = false,
                    ErrorMessage = "服务器内部错误，请稍后再试",
                    Data = null
                });
            }
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    ErrorMessage = "请求参数无效",
                    Data = null
                });
            }

            try
            {
                // 用户登录
                string username = request.UsernameOrEmail;
                
                // 如果输入是邮箱，尝试根据邮箱查找用户
                if (username.Contains("@"))
                {
                    var userByEmail = await _userRepository.GetByEmailAsync(username);
                    if (userByEmail != null)
                    {
                        username = userByEmail.Username;
                    }
                }
                
                var accessToken = await _authService.LoginAsync(username, request.Password);
                if (accessToken == null)
                {
                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        ErrorMessage = "用户名或密码错误",
                        Data = null
                    });
                }

                // 获取用户信息
                var user = await _userRepository.GetByUsernameAsync(username);
                
                // 创建刷新令牌
                var deviceId = request.DeviceInfo?.DeviceId ?? Guid.NewGuid().ToString();
                
                // 获取用户IP地址
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                
                // 创建用户令牌，传递IP地址
                var userToken = await _authService.CreateUserTokenAsync(user.Id, deviceId, ip);

                // 记录登录信息
                string deviceInfo = request.DeviceInfo?.DeviceName ?? "未知设备";
                await _authService.LogLoginAsync(user.Id, ip, deviceInfo);

                // 计算过期时间（秒）
                var expiresIn = (long)(userToken.RefreshTokenExpiresAt - DateTime.UtcNow).TotalSeconds;

                var authData = new AuthDataDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Nickname = user.Nickname,
                    AvatarUrl = user.AvatarUrl,
                    AccessToken = accessToken,
                    RefreshToken = userToken.RefreshToken,
                    ExpiresIn = expiresIn,
                    Roles = new[] { user.Role.ToString() },
                    Permissions = new string[] { } // 暂时不处理详细权限
                };

                return Ok(new AuthResponse
                {
                    Success = true,
                    Data = authData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户登录过程中发生错误");
                return StatusCode((int)HttpStatusCode.InternalServerError, new AuthResponse
                {
                    Success = false,
                    ErrorMessage = "服务器内部错误，请稍后再试",
                    Data = null
                });
            }
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(AuthResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    ErrorMessage = "请求参数无效",
                    Data = null
                });
            }

            try
            {
                // 刷新令牌
                var accessToken = await _authService.RefreshTokenAsync(request.RefreshToken);
                if (accessToken == null)
                {
                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        ErrorMessage = "无效的刷新令牌或令牌已过期",
                        Data = null
                    });
                }

                // 获取用户ID和令牌信息
                var userId = _authService.GetUserIdFromToken(accessToken);
                var user = await _userRepository.GetByIdAsync(userId);
                
                // 获取令牌信息
                var tokenRepository = _unitOfWork.Repository<UserToken>();
                var userToken = await tokenRepository.GetSingleAsync(t => t.RefreshToken == request.RefreshToken);
                
                // 计算过期时间（秒）
                var expiresIn = (long)(userToken.RefreshTokenExpiresAt - DateTime.UtcNow).TotalSeconds;

                var authData = new AuthDataDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Nickname = user.Nickname,
                    AvatarUrl = user.AvatarUrl,
                    AccessToken = accessToken,
                    RefreshToken = userToken.RefreshToken,
                    ExpiresIn = expiresIn,
                    Roles = new[] { user.Role.ToString() },
                    Permissions = new string[] { } // 暂时不处理详细权限
                };

                return Ok(new AuthResponse
                {
                    Success = true,
                    Data = authData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刷新令牌过程中发生错误");
                return StatusCode((int)HttpStatusCode.InternalServerError, new AuthResponse
                {
                    Success = false,
                    ErrorMessage = "服务器内部错误，请稍后再试",
                    Data = null
                });
            }
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                // 获取用户ID
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return BadRequest(new BaseResponse
                    {
                        Success = false,
                        ErrorMessage = "无效的用户凭证"
                    });
                }

                if (string.IsNullOrEmpty(request.DeviceId))
                {
                    // 如果没有指定设备ID，则撤销所有令牌
                    await _authService.RevokeAllTokensAsync(userId);
                }
                else
                {
                    // 撤销特定设备的令牌
                    var tokenRepository = _unitOfWork.Repository<UserToken>();
                    var token = await tokenRepository.GetSingleAsync(t => 
                        t.UserId == userId && t.DeviceId.ToString() == request.DeviceId);
                    
                    if (token != null)
                    {
                        await _authService.RevokeTokenAsync(token.RefreshToken);
                    }
                }

                return Ok(new BaseResponse 
                { 
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户注销过程中发生错误");
                return StatusCode((int)HttpStatusCode.InternalServerError, new BaseResponse
                {
                    Success = false,
                    ErrorMessage = "服务器内部错误，请稍后再试"
                });
            }
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(BaseResponse<UserDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                // 从令牌中获取用户ID
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return BadRequest(new BaseResponse<UserDto>
                    {
                        Success = false,
                        ErrorMessage = "无法获取用户信息",
                        Data = null
                    });
                }

                // 获取用户信息
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new BaseResponse<UserDto>
                    {
                        Success = false,
                        ErrorMessage = "用户不存在",
                        Data = null
                    });
                }

                // 映射到DTO - 根据UserDto中实际存在的属性进行映射
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Nickname = user.Nickname,
                    // 根据UserDto的实际定义映射属性
                    AvatarUrl = user.AvatarUrl,
                    Status = user.Status.ToString(),
                    LastOnlineTime = user.LastOnlineTime
                };

                return Ok(new BaseResponse<UserDto>
                {
                    Success = true,
                    Data = userDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取当前用户信息时发生错误");
                return StatusCode((int)HttpStatusCode.InternalServerError, new BaseResponse<UserDto>
                {
                    Success = false,
                    ErrorMessage = "服务器内部错误，请稍后再试",
                    Data = null
                });
            }
        }
    }
}