using IChat.Domain.Entities;
using IChat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// UserDevice 实体的仓储接口，继承自通用仓储接口，添加特定于用户设备的操作
    /// </summary>
    public interface IUserDeviceRepository : IRepository<UserDevice>
    {
        /// <summary>
        /// 获取用户的所有设备
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>设备列表</returns>
        Task<IEnumerable<UserDevice>> GetUserDevicesAsync(Guid userId);
        
        /// <summary>
        /// 获取用户的在线设备
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>在线设备列表</returns>
        Task<IEnumerable<UserDevice>> GetUserOnlineDevicesAsync(Guid userId);
        
        /// <summary>
        /// 根据设备标识获取设备
        /// </summary>
        /// <param name="deviceIdentifier">设备标识</param>
        /// <returns>设备，不存在则返回 null</returns>
        Task<UserDevice> GetByDeviceIdentifierAsync(string deviceIdentifier);
        
        /// <summary>
        /// 获取用户特定类型的设备
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="deviceType">设备类型</param>
        /// <returns>设备列表</returns>
        Task<IEnumerable<UserDevice>> GetUserDevicesByTypeAsync(Guid userId, DeviceType deviceType);
        
        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <param name="status">新状态</param>
        /// <returns>更新后的设备</returns>
        Task<UserDevice> UpdateDeviceStatusAsync(Guid deviceId, DeviceStatus status);
        
        /// <summary>
        /// 更新设备最后活跃时间
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <param name="lastActiveTime">最后活跃时间</param>
        /// <returns>更新后的设备</returns>
        Task<UserDevice> UpdateLastActiveTimeAsync(Guid deviceId, DateTime lastActiveTime);
        
        /// <summary>
        /// 更新设备推送令牌
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <param name="pushToken">推送令牌</param>
        /// <returns>更新后的设备</returns>
        Task<UserDevice> UpdatePushTokenAsync(Guid deviceId, string pushToken);
        
        /// <summary>
        /// 获取最近登录的设备
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>最近登录的设备</returns>
        Task<UserDevice> GetMostRecentDeviceAsync(Guid userId);
    }
}