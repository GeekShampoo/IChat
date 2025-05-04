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
    /// UserDevice实体的仓储实现类
    /// </summary>
    public class UserDeviceRepository : Repository<UserDevice>, IUserDeviceRepository
    {
        public UserDeviceRepository(IChatDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<UserDevice>> GetUserDevicesAsync(Guid userId)
        {
            return await _dbSet
                .Where(d => d.UserId == userId && !d.IsDeleted)
                .OrderByDescending(d => d.LastActiveTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserDevice>> GetUserOnlineDevicesAsync(Guid userId)
        {
            return await _dbSet
                .Where(d => d.UserId == userId && d.Status == DeviceStatus.Online && !d.IsDeleted)
                .OrderByDescending(d => d.LastActiveTime)
                .ToListAsync();
        }

        public async Task<UserDevice> GetByDeviceIdentifierAsync(string deviceIdentifier)
        {
            if (string.IsNullOrWhiteSpace(deviceIdentifier))
            {
                throw new ArgumentException("设备标识不能为空", nameof(deviceIdentifier));
            }

            return await _dbSet
                .FirstOrDefaultAsync(d => d.DeviceIdentifier == deviceIdentifier && !d.IsDeleted);
        }

        public async Task<IEnumerable<UserDevice>> GetUserDevicesByTypeAsync(Guid userId, DeviceType deviceType)
        {
            return await _dbSet
                .Where(d => d.UserId == userId && d.DeviceType == deviceType && !d.IsDeleted)
                .OrderByDescending(d => d.LastActiveTime)
                .ToListAsync();
        }

        public async Task<UserDevice> UpdateDeviceStatusAsync(Guid deviceId, DeviceStatus status)
        {
            var device = await _dbSet.FindAsync(deviceId);
            
            if (device == null)
            {
                throw new ArgumentException($"未找到ID为{deviceId}的设备", nameof(deviceId));
            }

            device.Status = status;
            device.UpdatedAt = DateTime.UtcNow;
            
            if (status == DeviceStatus.Online)
            {
                device.LastActiveTime = DateTime.UtcNow;
            }
            
            await _dbContext.SaveChangesAsync();
            return device;
        }

        public async Task<UserDevice> UpdateLastActiveTimeAsync(Guid deviceId, DateTime lastActiveTime)
        {
            var device = await _dbSet.FindAsync(deviceId);
            
            if (device == null)
            {
                throw new ArgumentException($"未找到ID为{deviceId}的设备", nameof(deviceId));
            }

            device.LastActiveTime = lastActiveTime;
            device.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            return device;
        }

        public async Task<UserDevice> UpdatePushTokenAsync(Guid deviceId, string pushToken)
        {
            var device = await _dbSet.FindAsync(deviceId);
            
            if (device == null)
            {
                throw new ArgumentException($"未找到ID为{deviceId}的设备", nameof(deviceId));
            }

            device.PushToken = pushToken;
            device.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            return device;
        }

        public async Task<UserDevice> GetMostRecentDeviceAsync(Guid userId)
        {
            return await _dbSet
                .Where(d => d.UserId == userId && !d.IsDeleted)
                .OrderByDescending(d => d.LastActiveTime)
                .FirstOrDefaultAsync();
        }
    }
}