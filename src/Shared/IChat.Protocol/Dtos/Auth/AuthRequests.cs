using System.ComponentModel.DataAnnotations;
using IChat.Protocol.Contracts;

namespace IChat.Protocol.Dtos.Auth
{
    /// <summary>
    /// 用户注册请求
    /// </summary>
    public class RegisterRequest : BaseRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Username { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Nickname { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        /// <summary>
        /// 电话号码（可选）
        /// </summary>
        [Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 设备信息
        /// </summary>
        public DeviceInfoDto DeviceInfo { get; set; }
    }

    /// <summary>
    /// 用户登录请求
    /// </summary>
    public class LoginRequest : BaseRequest
    {
        /// <summary>
        /// 用户名或电子邮箱
        /// </summary>
        [Required]
        public string UsernameOrEmail { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 是否记住登录状态
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// 设备信息
        /// </summary>
        public DeviceInfoDto DeviceInfo { get; set; }
    }

    /// <summary>
    /// 退出登录请求
    /// </summary>
    public class LogoutRequest : BaseRequest
    {
        /// <summary>
        /// 设备ID（可用于仅退出特定设备）
        /// </summary>
        public string DeviceId { get; set; }
    }

    /// <summary>
    /// 刷新令牌请求
    /// </summary>
    public class RefreshTokenRequest : BaseRequest
    {
        /// <summary>
        /// 刷新令牌
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// 设备信息DTO
    /// </summary>
    public class DeviceInfoDto
    {
        /// <summary>
        /// 设备唯一标识符
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// 操作系统
        /// </summary>
        public string OperatingSystem { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        public string AppVersion { get; set; }
    }
}