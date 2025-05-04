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
    /// Notification实体的仓储实现类
    /// </summary>
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(IChatDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(IEnumerable<Notification> Items, int TotalCount)> GetUserNotificationsAsync(Guid userId, int pageIndex, int pageSize)
        {
            IQueryable<Notification> query = _dbSet
                .Where(n => n.UserId == userId && !n.IsDeleted);

            int totalCount = await query.CountAsync();

            var pagedItems = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (pagedItems, totalCount);
        }

        public async Task<int> GetUnreadNotificationCountAsync(Guid userId)
        {
            return await _dbSet
                .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);
        }

        public async Task<(IEnumerable<Notification> Items, int TotalCount)> GetUserNotificationsByTypeAsync(
            Guid userId, 
            NotificationType notificationType, 
            int pageIndex, 
            int pageSize)
        {
            IQueryable<Notification> query = _dbSet
                .Where(n => n.UserId == userId && n.Type == notificationType && !n.IsDeleted);

            int totalCount = await query.CountAsync();

            var pagedItems = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (pagedItems, totalCount);
        }

        public async Task<bool> MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _dbSet.FindAsync(notificationId);
            
            if (notification == null)
            {
                throw new ArgumentException($"未找到ID为{notificationId}的通知", nameof(notificationId));
            }

            if (notification.IsRead)
            {
                return true; // 已经是已读状态，无需更改
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            notification.UpdatedAt = DateTime.UtcNow;
            
            int result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<int> MarkAllAsReadAsync(Guid userId)
        {
            var unreadNotifications = await _dbSet
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .ToListAsync();
                
            if (!unreadNotifications.Any())
            {
                return 0;
            }
            
            var now = DateTime.UtcNow;
            
            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = now;
                notification.UpdatedAt = now;
            }
            
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Notification> Items, int TotalCount)> GetUnreadNotificationsAsync(
            Guid userId, 
            int pageIndex, 
            int pageSize)
        {
            IQueryable<Notification> query = _dbSet
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);

            int totalCount = await query.CountAsync();

            var pagedItems = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (pagedItems, totalCount);
        }

        public async Task<bool> DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await _dbSet.FindAsync(notificationId);
            
            if (notification == null)
            {
                throw new ArgumentException($"未找到ID为{notificationId}的通知", nameof(notificationId));
            }

            notification.IsDeleted = true;
            notification.UpdatedAt = DateTime.UtcNow;
            
            int result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<int> AddNotificationsAsync(IEnumerable<Notification> notifications)
        {
            if (notifications == null || !notifications.Any())
            {
                return 0;
            }

            await _dbSet.AddRangeAsync(notifications);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CleanupExpiredNotificationsAsync(int expirationDays)
        {
            var expirationDate = DateTime.UtcNow.AddDays(-expirationDays);
            
            var expiredNotifications = await _dbSet
                .Where(n => n.CreatedAt < expirationDate && n.IsRead && !n.IsDeleted)
                .ToListAsync();
                
            if (!expiredNotifications.Any())
            {
                return 0;
            }
            
            var now = DateTime.UtcNow;
            
            foreach (var notification in expiredNotifications)
            {
                notification.IsDeleted = true;
                notification.UpdatedAt = now;
            }
            
            return await _dbContext.SaveChangesAsync();
        }
    }
}