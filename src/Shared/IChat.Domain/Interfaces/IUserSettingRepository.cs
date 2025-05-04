using IChat.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// UserSetting 实体的仓储接口，继承自通用仓储接口，添加特定于用户设置的操作
    /// </summary>
    public interface IUserSettingRepository : IRepository<UserSetting>
    {
        /// <summary>
        /// 获取用户的设置
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户设置，不存在则返回 null</returns>
        Task<UserSetting> GetUserSettingAsync(Guid userId);
        
        /// <summary>
        /// 更新用户特定设置项
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="settingKey">设置项键</param>
        /// <param name="settingValue">设置项值</param>
        /// <returns>更新后的用户设置</returns>
        Task<UserSetting> UpdateSettingItemAsync(Guid userId, string settingKey, string settingValue);
        
        /// <summary>
        /// 获取用户的隐私设置
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>隐私设置项</returns>
        Task<string> GetPrivacySettingsAsync(Guid userId);
        
        /// <summary>
        /// 更新用户的隐私设置
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="privacySettings">隐私设置项</param>
        /// <returns>更新后的用户设置</returns>
        Task<UserSetting> UpdatePrivacySettingsAsync(Guid userId, string privacySettings);
        
        /// <summary>
        /// 获取用户的通知设置
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>通知设置项</returns>
        Task<string> GetNotificationSettingsAsync(Guid userId);
        
        /// <summary>
        /// 更新用户的通知设置
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="notificationSettings">通知设置项</param>
        /// <returns>更新后的用户设置</returns>
        Task<UserSetting> UpdateNotificationSettingsAsync(Guid userId, string notificationSettings);
        
        /// <summary>
        /// 获取用户的主题设置
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>主题设置项</returns>
        Task<string> GetThemeSettingsAsync(Guid userId);
        
        /// <summary>
        /// 更新用户的主题设置
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="themeSettings">主题设置项</param>
        /// <returns>更新后的用户设置</returns>
        Task<UserSetting> UpdateThemeSettingsAsync(Guid userId, string themeSettings);
        
        /// <summary>
        /// 重置用户设置为默认值
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>重置后的用户设置</returns>
        Task<UserSetting> ResetToDefaultAsync(Guid userId);
    }
}