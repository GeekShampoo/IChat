using System;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 用户登录日志实体，记录用户的登录历史
    /// </summary>
    public class UserLoginLog : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 登录IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 登录设备ID
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// 登录位置（基于IP地址解析）
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 浏览器/客户端信息
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 是否登录成功
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// 失败原因（如果登录失败）
        /// </summary>
        public string FailureReason { get; set; }

        /// <summary>
        /// 关联的用户
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// 关联的设备
        /// </summary>
        public virtual UserDevice Device { get; set; }
    }
}