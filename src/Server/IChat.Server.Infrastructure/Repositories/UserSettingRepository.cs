using IChat.Domain.Entities;
using IChat.Domain.Interfaces;
using IChat.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Server.Infrastructure.Repositories
{
    /// <summary>
    /// UserSetting实体的仓储实现类
    /// </summary>
    public class UserSettingRepository : Repository<UserSetting>, IUserSettingRepository
    {
        public UserSettingRepository(IChatDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<UserSetting> GetUserSettingAsync(Guid userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
        }

        public async Task<UserSetting> UpdateSettingItemAsync(Guid userId, string settingKey, string settingValue)
        {
            if (string.IsNullOrWhiteSpace(settingKey))
            {
                throw new ArgumentException("设置键不能为空", nameof(settingKey));
            }

            var userSetting = await _dbSet
                .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
                
            if (userSetting == null)
            {
                // 用户设置不存在，创建新设置
                userSetting = new UserSetting
                {
                    UserId = userId
                };
                
                await _dbSet.AddAsync(userSetting);
            }
            
            // 根据settingKey更新对应的属性
            switch (settingKey)
            {
                case "themeMode":
                    userSetting.ThemeMode = int.Parse(settingValue);
                    break;
                case "themeColor":
                    userSetting.ThemeColor = settingValue;
                    break;
                case "enableSoundNotification":
                    userSetting.EnableSoundNotification = bool.Parse(settingValue);
                    break;
                case "enableNotification":
                    userSetting.EnableNotification = bool.Parse(settingValue);
                    break;
                case "showMessagePreview":
                    userSetting.ShowMessagePreview = bool.Parse(settingValue);
                    break;
                case "notificationSound":
                    userSetting.NotificationSound = settingValue;
                    break;
                case "autoDownloadFiles":
                    userSetting.AutoDownloadFiles = bool.Parse(settingValue);
                    break;
                case "autoDownloadMaxSize":
                    userSetting.AutoDownloadMaxSize = int.Parse(settingValue);
                    break;
                case "fontSize":
                    userSetting.FontSize = int.Parse(settingValue);
                    break;
                case "language":
                    userSetting.Language = settingValue;
                    break;
                case "autoLogin":
                    userSetting.AutoLogin = bool.Parse(settingValue);
                    break;
                case "notifyOnOtherDeviceLogin":
                    userSetting.NotifyOnOtherDeviceLogin = bool.Parse(settingValue);
                    break;
            }
            
            userSetting.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return userSetting;
        }

        public async Task<string> GetPrivacySettingsAsync(Guid userId)
        {
            var userSetting = await _dbSet
                .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
                
            if (userSetting == null)
            {
                return "{}"; // 返回默认空设置
            }
            
            // 将相关隐私设置属性组装为JSON返回
            var privacySettings = new Dictionary<string, object>
            {
                ["showMessagePreview"] = userSetting.ShowMessagePreview
            };
            
            return JsonConvert.SerializeObject(privacySettings);
        }

        public async Task<UserSetting> UpdatePrivacySettingsAsync(Guid userId, string privacySettings)
        {
            var userSetting = await _dbSet
                .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
                
            if (userSetting == null)
            {
                // 用户设置不存在，创建新设置
                userSetting = new UserSetting
                {
                    UserId = userId
                };
                
                await _dbSet.AddAsync(userSetting);
            }
            
            // 从JSON中解析隐私设置并更新对应属性
            var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(privacySettings);
            if (settings.ContainsKey("showMessagePreview"))
            {
                userSetting.ShowMessagePreview = Convert.ToBoolean(settings["showMessagePreview"]);
            }
            
            userSetting.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return userSetting;
        }

        public async Task<string> GetNotificationSettingsAsync(Guid userId)
        {
            var userSetting = await _dbSet
                .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
                
            if (userSetting == null)
            {
                return "{}"; // 返回默认空设置
            }
            
            // 将相关通知设置属性组装为JSON返回
            var notificationSettings = new Dictionary<string, object>
            {
                ["enableNotification"] = userSetting.EnableNotification,
                ["enableSoundNotification"] = userSetting.EnableSoundNotification,
                ["notificationSound"] = userSetting.NotificationSound,
                ["notifyOnOtherDeviceLogin"] = userSetting.NotifyOnOtherDeviceLogin
            };
            
            return JsonConvert.SerializeObject(notificationSettings);
        }

        public async Task<UserSetting> UpdateNotificationSettingsAsync(Guid userId, string notificationSettings)
        {
            var userSetting = await _dbSet
                .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
                
            if (userSetting == null)
            {
                // 用户设置不存在，创建新设置
                userSetting = new UserSetting
                {
                    UserId = userId
                };
                
                await _dbSet.AddAsync(userSetting);
            }
            
            // 从JSON中解析通知设置并更新对应属性
            var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(notificationSettings);
            
            if (settings.ContainsKey("enableNotification"))
            {
                userSetting.EnableNotification = Convert.ToBoolean(settings["enableNotification"]);
            }
            
            if (settings.ContainsKey("enableSoundNotification"))
            {
                userSetting.EnableSoundNotification = Convert.ToBoolean(settings["enableSoundNotification"]);
            }
            
            if (settings.ContainsKey("notificationSound"))
            {
                userSetting.NotificationSound = Convert.ToString(settings["notificationSound"]);
            }
            
            if (settings.ContainsKey("notifyOnOtherDeviceLogin"))
            {
                userSetting.NotifyOnOtherDeviceLogin = Convert.ToBoolean(settings["notifyOnOtherDeviceLogin"]);
            }
            
            userSetting.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return userSetting;
        }

        public async Task<string> GetThemeSettingsAsync(Guid userId)
        {
            var userSetting = await _dbSet
                .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
                
            if (userSetting == null)
            {
                return "{}"; // 返回默认空设置
            }
            
            // 将主题相关设置属性组装为JSON返回
            var themeSettings = new Dictionary<string, object>
            {
                ["themeMode"] = userSetting.ThemeMode,
                ["themeColor"] = userSetting.ThemeColor,
                ["fontSize"] = userSetting.FontSize
            };
            
            return JsonConvert.SerializeObject(themeSettings);
        }

        public async Task<UserSetting> UpdateThemeSettingsAsync(Guid userId, string themeSettings)
        {
            var userSetting = await _dbSet
                .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
                
            if (userSetting == null)
            {
                // 用户设置不存在，创建新设置
                userSetting = new UserSetting
                {
                    UserId = userId
                };
                
                await _dbSet.AddAsync(userSetting);
            }
            
            // 从JSON中解析主题设置并更新对应属性
            var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(themeSettings);
            
            if (settings.ContainsKey("themeMode"))
            {
                userSetting.ThemeMode = Convert.ToInt32(settings["themeMode"]);
            }
            
            if (settings.ContainsKey("themeColor"))
            {
                userSetting.ThemeColor = Convert.ToString(settings["themeColor"]);
            }
            
            if (settings.ContainsKey("fontSize"))
            {
                userSetting.FontSize = Convert.ToInt32(settings["fontSize"]);
            }
            
            userSetting.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return userSetting;
        }

        public async Task<UserSetting> ResetToDefaultAsync(Guid userId)
        {
            var userSetting = await _dbSet
                .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
                
            if (userSetting == null)
            {
                // 用户设置不存在，创建新设置
                userSetting = new UserSetting
                {
                    UserId = userId,
                    ThemeMode = 0,
                    ThemeColor = "#1890ff",
                    EnableSoundNotification = true,
                    EnableNotification = true,
                    ShowMessagePreview = true,
                    NotificationSound = "default",
                    AutoDownloadFiles = false,
                    AutoDownloadMaxSize = 10,
                    FontSize = 1,
                    Language = "zh-CN",
                    AutoLogin = true,
                    NotifyOnOtherDeviceLogin = true
                };
                
                await _dbSet.AddAsync(userSetting);
            }
            else
            {
                // 重置为默认设置
                userSetting.ThemeMode = 0;
                userSetting.ThemeColor = "#1890ff";
                userSetting.EnableSoundNotification = true;
                userSetting.EnableNotification = true;
                userSetting.ShowMessagePreview = true;
                userSetting.NotificationSound = "default";
                userSetting.AutoDownloadFiles = false;
                userSetting.AutoDownloadMaxSize = 10;
                userSetting.FontSize = 1;
                userSetting.Language = "zh-CN";
                userSetting.AutoLogin = true;
                userSetting.NotifyOnOtherDeviceLogin = true;
                userSetting.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
            return userSetting;
        }
    }
}