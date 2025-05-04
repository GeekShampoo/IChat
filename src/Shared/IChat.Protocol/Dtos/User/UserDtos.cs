using System;

namespace IChat.Protocol.Dtos.User
{
    /// <summary>
    /// 用户信息DTO
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 个人签名
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 在线状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 上次在线时间
        /// </summary>
        public DateTime? LastOnlineTime { get; set; }

        /// <summary>
        /// 是否为好友（当前用户视角）
        /// </summary>
        public bool IsFriend { get; set; }

        /// <summary>
        /// 好友备注（当前用户视角）
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 用户详细信息DTO
    /// </summary>
    public class UserDetailDto : UserDto
    {
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 是否已验证邮箱
        /// </summary>
        public bool IsEmailVerified { get; set; }

        /// <summary>
        /// 是否已验证手机
        /// </summary>
        public bool IsPhoneVerified { get; set; }
    }

    /// <summary>
    /// 用户更新信息DTO
    /// </summary>
    public class UpdateUserInfoDto
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 个人签名
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// 更改密码DTO
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// 当前密码
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// 确认新密码
        /// </summary>
        public string ConfirmNewPassword { get; set; }
    }
}