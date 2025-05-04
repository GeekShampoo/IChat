using IChat.Domain.Entities;
using IChat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// Notification 实体的仓储接口，继承自通用仓储接口，添加特定于通知的操作
    /// </summary>
    public interface INotificationRepository : IRepository<Notification>
    {
        /// <summary>
        /// 获取用户的所有通知
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pageIndex">页码，从 1 开始</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>分页后的通知列表和总数</returns>
        Task<(IEnumerable<Notification> Items, int TotalCount)> GetUserNotificationsAsync(Guid userId, int pageIndex, int pageSize);
        
        /// <summary>
        /// 获取用户未读通知数量
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>未读通知数量</returns>
        Task<int> GetUnreadNotificationCountAsync(Guid userId);
        
        /// <summary>
        /// 获取用户特定类型的通知
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="notificationType">通知类型</param>
        /// <param name="pageIndex">页码，从 1 开始</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>分页后的通知列表和总数</returns>
        Task<(IEnumerable<Notification> Items, int TotalCount)> GetUserNotificationsByTypeAsync(
            Guid userId, 
            NotificationType notificationType, 
            int pageIndex, 
            int pageSize);
        
        /// <summary>
        /// 将通知标记为已读
        /// </summary>
        /// <param name="notificationId">通知ID</param>
        /// <returns>操作是否成功</returns>
        Task<bool> MarkAsReadAsync(Guid notificationId);
        
        /// <summary>
        /// 将用户的所有通知标记为已读
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>标记为已读的通知数量</returns>
        Task<int> MarkAllAsReadAsync(Guid userId);
        
        /// <summary>
        /// 获取用户未读通知
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pageIndex">页码，从 1 开始</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>分页后的未读通知列表和总数</returns>
        Task<(IEnumerable<Notification> Items, int TotalCount)> GetUnreadNotificationsAsync(
            Guid userId, 
            int pageIndex, 
            int pageSize);
        
        /// <summary>
        /// 删除通知
        /// </summary>
        /// <param name="notificationId">通知ID</param>
        /// <returns>操作是否成功</returns>
        Task<bool> DeleteNotificationAsync(Guid notificationId);
        
        /// <summary>
        /// 批量添加通知
        /// </summary>
        /// <param name="notifications">通知列表</param>
        /// <returns>添加的通知数量</returns>
        Task<int> AddNotificationsAsync(IEnumerable<Notification> notifications);
        
        /// <summary>
        /// 清理过期通知
        /// </summary>
        /// <param name="expirationDays">过期天数</param>
        /// <returns>清理的通知数量</returns>
        Task<int> CleanupExpiredNotificationsAsync(int expirationDays);
    }
}