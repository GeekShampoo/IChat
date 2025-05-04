using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IChat.Domain.Enums;
using IChat.Protocol.Dtos.Message;
using IChat.Protocol.Messages;
using IChat.Protocol.Messages.Chat;
using IChat.Protocol.Messages.Presence;
using IChat.Protocol.Messages.System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using IChat.Server.Core.Interfaces;
using IChat.Domain.Entities;

namespace IChat.Server.Web.Hubs
{
    /// <summary>
    /// 处理实时消息通信的SignalR Hub
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly IConnectionManager _connectionManager;
        private readonly IMessageService _messageService;

        // 注意：根据项目进展，后续需要注入更多服务
        // private readonly IUserService _userService;
        // private readonly IGroupService _groupService;

        public ChatHub(
            ILogger<ChatHub> logger,
            IConnectionManager connectionManager,
            IMessageService messageService)
        {
            _logger = logger;
            _connectionManager = connectionManager;
            _messageService = messageService;
        }

        /// <summary>
        /// 当客户端连接时调用
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            var connectionId = Context.ConnectionId;

            // 记录用户连接
            _connectionManager.AddConnection(userId, connectionId);
            _logger.LogInformation($"用户 {userId} 已连接，连接ID: {connectionId}");

            // 通知用户连接成功
            await Clients.Caller.SendAsync("ReceiveMessage", 
                new ConnectionStatusMessage
                {
                    Status = "Connected",
                    ConnectionId = connectionId,
                    ServerTime = DateTime.UtcNow,
                    Details = "连接已建立"
                });

            // 通知用户的好友该用户已上线（后续实现）
            // await NotifyUserPresenceChange(userId, "Online");

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 当客户端断开连接时调用
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = GetUserId();
            var connectionId = Context.ConnectionId;

            // 移除用户连接
            _connectionManager.RemoveConnection(userId, connectionId);
            _logger.LogInformation($"用户 {userId} 已断开连接，连接ID: {connectionId}");

            // 如果用户所有连接都断开，通知好友该用户已离线（后续实现）
            if (!_connectionManager.HasActiveConnections(userId))
            {
                // await NotifyUserPresenceChange(userId, "Offline");
            }

            await base.OnDisconnectedAsync(exception);
        }

        #region 消息发送接口

        /// <summary>
        /// 发送私聊消息
        /// </summary>
        public async Task SendPrivateMessage(SendMessageRequest request)
        {
            if (request.RecipientId == null)
            {
                await SendErrorToClient("接收者ID不能为空");
                return;
            }

            var senderId = GetUserId();
            var senderConnectionId = Context.ConnectionId;

            try
            {
                // 验证消息内容（后续可扩展）
                if (string.IsNullOrEmpty(request.Content))
                {
                    await SendErrorToClient("消息内容不能为空");
                    return;
                }

                // 创建消息（后续会调用消息服务保存到数据库）
                var messageDto = new MessageDto
                {
                    Id = Guid.NewGuid(),
                    SenderId = senderId,
                    SenderName = GetUsername(),
                    SenderAvatar = "", // 后续实现
                    RecipientId = request.RecipientId,
                    Type = request.Type,
                    Content = request.Content,
                    ReplyToMessageId = request.ReplyToMessageId,
                    Status = MessageStatus.Sent.ToString(),
                    SendTime = DateTime.UtcNow,
                    ExtendedData = request.ExtendedData
                };

                // 创建消息接收事件
                var messageEvent = new ChatMessageReceivedMessage
                {
                    Message = messageDto
                };

                // 向发送者发送消息状态更新，表示消息已发送
                await Clients.Caller.SendAsync("ReceiveMessage", new MessageStatusUpdatedMessage
                {
                    MessageId = messageDto.Id,
                    ClientMessageId = request.ClientMessageId,
                    NewStatus = MessageStatus.Sent.ToString(),
                    UpdatedAt = DateTime.UtcNow
                });

                // 获取接收者的连接ID列表
                var recipientConnections = _connectionManager.GetUserConnections(request.RecipientId.Value);

                if (recipientConnections.Any())
                {
                    // 接收者在线，直接发送消息
                    await Clients.Clients(recipientConnections).SendAsync("ReceiveMessage", messageEvent);

                    // 更新发送者的消息状态为已送达
                    await Clients.Caller.SendAsync("ReceiveMessage", new MessageStatusUpdatedMessage
                    {
                        MessageId = messageDto.Id,
                        ClientMessageId = request.ClientMessageId,
                        NewStatus = MessageStatus.Delivered.ToString(),
                        UpdatedAt = DateTime.UtcNow
                    });

                    // 同时发送新消息通知
                    await Clients.Clients(recipientConnections).SendAsync("ReceiveMessage", new NewMessageNotificationMessage
                    {
                        SenderId = senderId,
                        SenderName = GetUsername(),
                        ConversationType = "Private",
                        TargetId = senderId,
                        TargetName = GetUsername(),
                        MessageType = request.Type,
                        MessagePreview = TruncateContent(request.Content, 50),
                        SentAt = DateTime.UtcNow,
                        UnreadCount = 1 // 未读数量，后续需要从数据库获取
                    });
                }
                else
                {
                    // 接收者离线，消息将保存在数据库中等待接收者上线后推送
                    // 后续实现离线消息存储和推送机制
                    _logger.LogInformation($"用户 {request.RecipientId} 当前离线，消息将作为离线消息存储");
                }

                // 记录日志
                _logger.LogInformation($"用户 {senderId} 向用户 {request.RecipientId} 发送了类型为 {request.Type} 的消息");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"发送私聊消息失败: {ex.Message}");
                await SendErrorToClient($"发送消息失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 发送群聊消息
        /// </summary>
        public async Task SendGroupMessage(SendMessageRequest request)
        {
            if (request.GroupId == null)
            {
                await SendErrorToClient("群组ID不能为空");
                return;
            }

            var senderId = GetUserId();
            var senderConnectionId = Context.ConnectionId;

            try
            {
                // 验证消息内容
                if (string.IsNullOrEmpty(request.Content))
                {
                    await SendErrorToClient("消息内容不能为空");
                    return;
                }

                // 验证用户是否在群组中（后续实现）
                // bool isGroupMember = await _groupService.IsUserInGroupAsync(senderId, request.GroupId.Value);
                // if (!isGroupMember)
                // {
                //     await SendErrorToClient("您不是该群组的成员，无法发送消息");
                //     return;
                // }

                // 创建消息（后续会调用消息服务保存到数据库）
                var messageDto = new MessageDto
                {
                    Id = Guid.NewGuid(),
                    SenderId = senderId,
                    SenderName = GetUsername(),
                    SenderAvatar = "", // 后续实现
                    GroupId = request.GroupId,
                    Type = request.Type,
                    Content = request.Content,
                    ReplyToMessageId = request.ReplyToMessageId,
                    Status = MessageStatus.Sent.ToString(),
                    SendTime = DateTime.UtcNow,
                    ExtendedData = request.ExtendedData
                };

                // 创建消息接收事件
                var messageEvent = new ChatMessageReceivedMessage
                {
                    Message = messageDto
                };

                // 向发送者发送消息状态更新，表示消息已发送
                await Clients.Caller.SendAsync("ReceiveMessage", new MessageStatusUpdatedMessage
                {
                    MessageId = messageDto.Id,
                    ClientMessageId = request.ClientMessageId,
                    NewStatus = MessageStatus.Sent.ToString(),
                    UpdatedAt = DateTime.UtcNow
                });

                // 获取群组成员列表并广播消息（后续实现）
                // var groupMembers = await _groupService.GetGroupMembersAsync(request.GroupId.Value);
                // var groupName = await _groupService.GetGroupNameAsync(request.GroupId.Value);
                
                // 临时使用SignalR组广播（实际应用中可能需要更复杂的逻辑）
                string groupName = $"Group_{request.GroupId}";
                
                // 向群组成员广播消息
                await Clients.Group(groupName).SendAsync("ReceiveMessage", messageEvent);

                // 更新发送者的消息状态为已送达
                await Clients.Caller.SendAsync("ReceiveMessage", new MessageStatusUpdatedMessage
                {
                    MessageId = messageDto.Id,
                    ClientMessageId = request.ClientMessageId,
                    NewStatus = MessageStatus.Delivered.ToString(),
                    UpdatedAt = DateTime.UtcNow
                });

                // 同时发送新消息通知（后续可优化为只发给不在当前会话窗口的成员）
                await Clients.Group(groupName).SendAsync("ReceiveMessage", new NewMessageNotificationMessage
                {
                    SenderId = senderId,
                    SenderName = GetUsername(),
                    ConversationType = "Group",
                    TargetId = request.GroupId.Value,
                    TargetName = groupName, // 后续应该从数据库获取群组名称
                    MessageType = request.Type,
                    MessagePreview = TruncateContent(request.Content, 50),
                    SentAt = DateTime.UtcNow,
                    UnreadCount = 1 // 未读数量，后续需要从数据库获取
                });

                // 记录日志
                _logger.LogInformation($"用户 {senderId} 向群组 {request.GroupId} 发送了类型为 {request.Type} 的消息");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"发送群聊消息失败: {ex.Message}");
                await SendErrorToClient($"发送消息失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 标记消息为已读
        /// </summary>
        public async Task MarkMessagesAsRead(MessageReadNotification request)
        {
            if (request.MessageIds == null || !request.MessageIds.Any())
            {
                await SendErrorToClient("消息ID列表不能为空");
                return;
            }

            var userId = GetUserId();
            var username = GetUsername();

            try
            {
                // 创建消息已读通知
                var readMessage = new MessagesReadMessage
                {
                    ReadByUserId = userId,
                    ReadByUserName = username,
                    MessageIds = request.MessageIds,
                    ConversationType = request.ConversationType,
                    TargetId = request.TargetId,
                    ReadAt = DateTime.UtcNow
                };

                if (request.ConversationType.Equals("Private", StringComparison.OrdinalIgnoreCase))
                {
                    // 私聊：通知消息发送者
                    var targetConnectionIds = _connectionManager.GetUserConnections(request.TargetId);
                    if (targetConnectionIds.Any())
                    {
                        await Clients.Clients(targetConnectionIds).SendAsync("ReceiveMessage", readMessage);
                    }
                }
                else if (request.ConversationType.Equals("Group", StringComparison.OrdinalIgnoreCase))
                {
                    // 群聊：通知群组内所有成员（可能需要优化为只通知消息发送者）
                    string groupName = $"Group_{request.TargetId}";
                    await Clients.Group(groupName).SendAsync("ReceiveMessage", readMessage);
                }

                // 后续实现：在数据库中更新消息已读状态
                // await _messageService.MarkMessagesAsReadAsync(userId, request.MessageIds);

                _logger.LogInformation($"用户 {userId} 已将 {request.ConversationType} {request.TargetId} 中的 {request.MessageIds.Count} 条消息标记为已读");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"标记消息为已读失败: {ex.Message}");
                await SendErrorToClient($"标记消息已读失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 撤回消息
        /// </summary>
        public async Task RecallMessage(RecallMessageRequest request)
        {
            var userId = GetUserId();
            var username = GetUsername();

            try
            {
                // 验证消息是否存在及是否可撤回（后续实现）
                // 通常只允许撤回自己发送的、一定时间内的消息
                // var message = await _messageService.GetMessageByIdAsync(request.MessageId);
                // if (message == null)
                // {
                //     await SendErrorToClient("消息不存在");
                //     return;
                // }
                // 
                // if (message.SenderId != userId)
                // {
                //     await SendErrorToClient("您只能撤回自己发送的消息");
                //     return;
                // }
                // 
                // var messageAge = DateTime.UtcNow - message.SendTime;
                // if (messageAge.TotalMinutes > 2)
                // {
                //     await SendErrorToClient("消息发送超过2分钟，无法撤回");
                //     return;
                // }

                // 创建消息撤回通知
                var recallMessage = new MessageRecalledMessage
                {
                    MessageId = request.MessageId,
                    ConversationType = "Private", // 后续应从数据库获取
                    TargetId = Guid.Empty, // 后续应从数据库获取
                    OperatorId = userId,
                    OperatorName = username,
                    RecalledAt = DateTime.UtcNow
                };

                // 向相关用户发送撤回通知（后续完善）
                // 这里暂时模拟为私聊场景，群聊场景类似
                Guid recipientId = Guid.Empty; // 后续应从数据库获取
                var recipientConnections = _connectionManager.GetUserConnections(recipientId);
                
                // 通知自己
                await Clients.Caller.SendAsync("ReceiveMessage", recallMessage);
                
                // 通知对方（如果在线）
                if (recipientConnections.Any())
                {
                    await Clients.Clients(recipientConnections).SendAsync("ReceiveMessage", recallMessage);
                }

                // 后续实现：在数据库中更新消息状态为已撤回
                // await _messageService.RecallMessageAsync(request.MessageId);

                _logger.LogInformation($"用户 {userId} 已撤回消息 {request.MessageId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"撤回消息失败: {ex.Message}");
                await SendErrorToClient($"撤回消息失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 发送正在输入状态通知
        /// </summary>
        public async Task SendTypingNotification(Guid targetId, string conversationType)
        {
            var userId = GetUserId();
            var username = GetUsername();

            try
            {
                var notification = new TypingNotificationMessage
                {
                    UserId = userId,
                    UserName = username,
                    ConversationType = conversationType,
                    TargetId = targetId
                };

                if (conversationType.Equals("Private", StringComparison.OrdinalIgnoreCase))
                {
                    // 私聊：只通知目标用户
                    var targetConnections = _connectionManager.GetUserConnections(targetId);
                    if (targetConnections.Any())
                    {
                        await Clients.Clients(targetConnections).SendAsync("ReceiveMessage", notification);
                    }
                }
                else if (conversationType.Equals("Group", StringComparison.OrdinalIgnoreCase))
                {
                    // 群聊：通知群组内所有其他成员
                    string groupName = $"Group_{targetId}";
                    await Clients.OthersInGroup(groupName).SendAsync("ReceiveMessage", notification);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"发送正在输入状态通知失败: {ex.Message}");
            }
        }

        #endregion

        #region 群组管理接口

        /// <summary>
        /// 加入群组聊天
        /// </summary>
        public async Task JoinGroup(Guid groupId)
        {
            var userId = GetUserId();
            var username = GetUsername();

            try
            {
                // 验证用户是否有权限加入群组（后续实现）
                // bool canJoin = await _groupService.CanUserJoinGroupAsync(userId, groupId);
                // if (!canJoin)
                // {
                //     await SendErrorToClient("您不是该群组的成员，无法加入");
                //     return;
                // }

                // 加入SignalR群组
                string groupName = $"Group_{groupId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                _logger.LogInformation($"用户 {userId} 已加入群组 {groupId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"加入群组失败: {ex.Message}");
                await SendErrorToClient($"加入群组失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 离开群组聊天
        /// </summary>
        public async Task LeaveGroup(Guid groupId)
        {
            var userId = GetUserId();

            try
            {
                // 离开SignalR群组
                string groupName = $"Group_{groupId}";
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

                _logger.LogInformation($"用户 {userId} 已离开群组 {groupId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"离开群组失败: {ex.Message}");
                await SendErrorToClient($"离开群组失败: {ex.Message}");
            }
        }

        #endregion

        #region 状态管理接口

        /// <summary>
        /// 更新用户在线状态
        /// </summary>
        public async Task UpdateUserStatus(string status, string statusMessage = "")
        {
            var userId = GetUserId();
            var username = GetUsername();

            try
            {
                // 更新用户状态（后续实现）
                // await _userService.UpdateUserStatusAsync(userId, status, statusMessage);

                // 通知该用户的所有好友状态已更改（后续实现）
                // var friends = await _userService.GetUserFriendsAsync(userId);
                // foreach (var friend in friends)
                // {
                //     var friendConnections = _connectionManager.GetUserConnections(friend.Id);
                //     if (friendConnections.Any())
                //     {
                //         await Clients.Clients(friendConnections).SendAsync("ReceiveMessage", new UserPresenceUpdatedMessage
                //         {
                //             UserId = userId,
                //             Username = username,
                //             Status = status,
                //             StatusMessage = statusMessage,
                //             LastActiveTime = DateTime.UtcNow,
                //             DeviceInfo = Context.GetHttpContext()?.Request.Headers["User-Agent"].ToString() ?? "Unknown"
                //         });
                //     }
                // }

                _logger.LogInformation($"用户 {userId} 已更新状态为 {status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新用户状态失败: {ex.Message}");
                await SendErrorToClient($"更新状态失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取好友在线状态
        /// </summary>
        public async Task GetFriendsStatus()
        {
            var userId = GetUserId();

            try
            {
                // 获取好友及其状态（后续实现）
                // var friends = await _userService.GetUserFriendsWithStatusAsync(userId);
                // 
                // var presenceMessage = new FriendsPresenceMessage
                // {
                //     OnlineFriends = friends.Select(f => new FriendPresenceInfo
                //     {
                //         UserId = f.Id,
                //         Username = f.Username,
                //         Status = f.Status,
                //         StatusMessage = f.StatusMessage,
                //         LastActiveTime = f.LastActiveTime,
                //         DeviceType = f.LastDeviceType
                //     }).ToList()
                // };
                // 
                // await Clients.Caller.SendAsync("ReceiveMessage", presenceMessage);

                _logger.LogInformation($"用户 {userId} 请求获取好友在线状态");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取好友状态失败: {ex.Message}");
                await SendErrorToClient($"获取好友状态失败: {ex.Message}");
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        private Guid GetUserId()
        {
            var userIdClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 获取当前用户名
        /// </summary>
        private string GetUsername()
        {
            return Context.User.FindFirst(ClaimTypes.Name)?.Value ?? "未知用户";
        }

        /// <summary>
        /// 发送错误消息给客户端
        /// </summary>
        private async Task SendErrorToClient(string errorMessage)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", new ErrorMessage
            {
                ErrorCode = "400",
                ErrorDescription = errorMessage,
            });
        }

        /// <summary>
        /// 截断消息内容
        /// </summary>
        private string TruncateContent(string content, int maxLength)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;

            return content.Length <= maxLength ? content : content.Substring(0, maxLength) + "...";
        }

        #endregion
    }

    /// <summary>
    /// 连接管理器接口，负责管理用户与连接ID的映射关系
    /// </summary>
    public interface IConnectionManager
    {
        void AddConnection(Guid userId, string connectionId);
        void RemoveConnection(Guid userId, string connectionId);
        List<string> GetUserConnections(Guid userId);
        bool HasActiveConnections(Guid userId);
    }

    /// <summary>
    /// 连接管理器实现类
    /// </summary>
    public class ConnectionManager : IConnectionManager
    {
        // 用户ID -> 连接ID列表的映射
        private readonly Dictionary<Guid, HashSet<string>> _userConnections = new Dictionary<Guid, HashSet<string>>();
        private readonly object _lock = new object();

        public void AddConnection(Guid userId, string connectionId)
        {
            lock (_lock)
            {
                if (!_userConnections.TryGetValue(userId, out var connections))
                {
                    connections = new HashSet<string>();
                    _userConnections[userId] = connections;
                }
                connections.Add(connectionId);
            }
        }

        public void RemoveConnection(Guid userId, string connectionId)
        {
            lock (_lock)
            {
                if (_userConnections.TryGetValue(userId, out var connections))
                {
                    connections.Remove(connectionId);
                    if (connections.Count == 0)
                    {
                        _userConnections.Remove(userId);
                    }
                }
            }
        }

        public List<string> GetUserConnections(Guid userId)
        {
            lock (_lock)
            {
                if (_userConnections.TryGetValue(userId, out var connections))
                {
                    return connections.ToList();
                }
                return new List<string>();
            }
        }

        public bool HasActiveConnections(Guid userId)
        {
            lock (_lock)
            {
                return _userConnections.TryGetValue(userId, out var connections) && connections.Count > 0;
            }
        }
    }
}