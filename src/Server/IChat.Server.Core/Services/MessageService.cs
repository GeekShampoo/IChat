using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IChat.Domain.Entities;
using IChat.Domain.Enums;
using IChat.Domain.Interfaces;
using IChat.Protocol.Dtos.Message;
using IChat.Server.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IChat.Server.Core.Services
{
    /// <summary>
    /// 消息服务实现类，提供消息相关的业务逻辑
    /// </summary>
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MessageService> _logger;

        public MessageService(
            IMessageRepository messageRepository,
            IUnitOfWork unitOfWork,
            ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 发送私聊消息
        /// </summary>
        public async Task<Message> SendPrivateMessageAsync(
            Guid senderId, 
            Guid recipientId, 
            MessageType messageType, 
            string content, 
            Guid? replyToMessageId = null, 
            Guid? clientMessageId = null,
            string extendedData = null)
        {
            try
            {
                // 验证参数
                if (senderId == Guid.Empty)
                    throw new ArgumentException("发送者ID不能为空", nameof(senderId));
                if (recipientId == Guid.Empty)
                    throw new ArgumentException("接收者ID不能为空", nameof(recipientId));
                if (string.IsNullOrWhiteSpace(content))
                    throw new ArgumentException("消息内容不能为空", nameof(content));

                // 创建消息实体
                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    SenderId = senderId,
                    RecipientId = recipientId,
                    GroupId = null, // 私聊消息没有群组ID
                    Type = messageType,
                    Content = content,
                    ReplyToMessageId = replyToMessageId,
                    Status = MessageStatus.Sent, // 初始状态为已发送
                    SendTime = DateTime.UtcNow,
                    ExtendedData = extendedData
                };

                // 使用工作单元保存消息
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var savedMessage = await _messageRepository.AddAsync(message);
                    return savedMessage;
                });

                _logger.LogInformation($"私聊消息发送成功: 从 {senderId} 到 {recipientId}, 消息ID: {message.Id}");
                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"发送私聊消息失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 发送群组消息
        /// </summary>
        public async Task<Message> SendGroupMessageAsync(
            Guid senderId, 
            Guid groupId, 
            MessageType messageType, 
            string content, 
            Guid? replyToMessageId = null, 
            Guid? clientMessageId = null,
            string extendedData = null)
        {
            try
            {
                // 验证参数
                if (senderId == Guid.Empty)
                    throw new ArgumentException("发送者ID不能为空", nameof(senderId));
                if (groupId == Guid.Empty)
                    throw new ArgumentException("群组ID不能为空", nameof(groupId));
                if (string.IsNullOrWhiteSpace(content))
                    throw new ArgumentException("消息内容不能为空", nameof(content));

                // 创建消息实体
                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    SenderId = senderId,
                    RecipientId = null, // 群聊消息没有特定接收者
                    GroupId = groupId,
                    Type = messageType,
                    Content = content,
                    ReplyToMessageId = replyToMessageId,
                    Status = MessageStatus.Sent, // 初始状态为已发送
                    SendTime = DateTime.UtcNow,
                    ExtendedData = extendedData
                };

                // 使用工作单元保存消息
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var savedMessage = await _messageRepository.AddAsync(message);
                    return savedMessage;
                });

                _logger.LogInformation($"群组消息发送成功: 从 {senderId} 到群组 {groupId}, 消息ID: {message.Id}");
                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"发送群组消息失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取私聊消息历史
        /// </summary>
        public async Task<(IEnumerable<MessageDto> Messages, string NextPageToken)> GetPrivateMessageHistoryAsync(
            Guid user1Id, 
            Guid user2Id, 
            string pageToken = null, 
            int pageSize = 20,
            string direction = "Backward")
        {
            try
            {
                // 验证参数
                if (user1Id == Guid.Empty)
                    throw new ArgumentException("用户1 ID不能为空", nameof(user1Id));
                if (user2Id == Guid.Empty)
                    throw new ArgumentException("用户2 ID不能为空", nameof(user2Id));
                if (pageSize <= 0)
                    pageSize = 20;

                // 解析分页令牌
                Guid? beforeMessageId = null;
                if (!string.IsNullOrEmpty(pageToken))
                {
                    try
                    {
                        beforeMessageId = Guid.Parse(pageToken);
                    }
                    catch
                    {
                        _logger.LogWarning($"无效的分页令牌: {pageToken}");
                    }
                }

                // 获取消息历史
                var messages = await _messageRepository.GetPrivateMessageHistoryAsync(
                    user1Id,
                    user2Id,
                    beforeMessageId,
                    pageSize + 1); // 多获取一条用于判断是否有下一页

                var messagesList = messages.ToList();
                string nextPageToken = null;

                // 如果实际获取的消息数量超过请求的数量，说明还有更多消息
                if (messagesList.Count > pageSize)
                {
                    // 取出最后一条消息的ID作为下一页的令牌
                    var lastMessage = messagesList.Last();
                    nextPageToken = lastMessage.Id.ToString();

                    // 移除多余的消息
                    messagesList = messagesList.Take(pageSize).ToList();
                }

                // 转换为DTO
                var messageDtos = messagesList.Select(MapMessageToDto).ToList();

                return (messageDtos, nextPageToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取私聊消息历史失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取群组消息历史
        /// </summary>
        public async Task<(IEnumerable<MessageDto> Messages, string NextPageToken)> GetGroupMessageHistoryAsync(
            Guid userId,
            Guid groupId, 
            string pageToken = null, 
            int pageSize = 20,
            string direction = "Backward")
        {
            try
            {
                // 验证参数
                if (userId == Guid.Empty)
                    throw new ArgumentException("用户ID不能为空", nameof(userId));
                if (groupId == Guid.Empty)
                    throw new ArgumentException("群组ID不能为空", nameof(groupId));
                if (pageSize <= 0)
                    pageSize = 20;

                // 解析分页令牌
                Guid? beforeMessageId = null;
                if (!string.IsNullOrEmpty(pageToken))
                {
                    try
                    {
                        beforeMessageId = Guid.Parse(pageToken);
                    }
                    catch
                    {
                        _logger.LogWarning($"无效的分页令牌: {pageToken}");
                    }
                }

                // 获取消息历史
                var messages = await _messageRepository.GetGroupMessageHistoryAsync(
                    groupId,
                    beforeMessageId,
                    pageSize + 1); // 多获取一条用于判断是否有下一页

                var messagesList = messages.ToList();
                string nextPageToken = null;

                // 如果实际获取的消息数量超过请求的数量，说明还有更多消息
                if (messagesList.Count > pageSize)
                {
                    // 取出最后一条消息的ID作为下一页的令牌
                    var lastMessage = messagesList.Last();
                    nextPageToken = lastMessage.Id.ToString();

                    // 移除多余的消息
                    messagesList = messagesList.Take(pageSize).ToList();
                }

                // 转换为DTO
                var messageDtos = messagesList.Select(MapMessageToDto).ToList();

                // 标记当前用户已读的消息
                if (messageDtos.Any())
                {
                    // 获取该用户的已读回执
                    var messageIds = messageDtos.Select(m => m.Id).ToList();
                    
                    foreach (var messageDto in messageDtos)
                    {
                        // 如果是当前用户发送的消息，标记为已读
                        if (messageDto.SenderId == userId)
                        {
                            messageDto.Status = MessageStatus.Read.ToString();
                        }
                    }
                }

                return (messageDtos, nextPageToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取群组消息历史失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 标记消息为已读
        /// </summary>
        public async Task<bool> MarkMessagesAsReadAsync(Guid userId, IEnumerable<Guid> messageIds)
        {
            try
            {
                // 验证参数
                if (userId == Guid.Empty)
                    throw new ArgumentException("用户ID不能为空", nameof(userId));
                if (messageIds == null || !messageIds.Any())
                    throw new ArgumentException("消息ID列表不能为空", nameof(messageIds));

                // 标记消息为已读
                var result = await _messageRepository.MarkMessagesAsReadAsync(messageIds, userId);
                
                if (result)
                {
                    _logger.LogInformation($"用户 {userId} 标记 {messageIds.Count()} 条消息为已读");
                }
                else
                {
                    _logger.LogWarning($"用户 {userId} 标记消息为已读失败");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"标记消息为已读失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 标记会话所有消息为已读
        /// </summary>
        public async Task<bool> MarkConversationAsReadAsync(Guid userId, ConversationType conversationType, Guid targetId)
        {
            try
            {
                // 验证参数
                if (userId == Guid.Empty)
                    throw new ArgumentException("用户ID不能为空", nameof(userId));
                if (targetId == Guid.Empty)
                    throw new ArgumentException("目标ID不能为空", nameof(targetId));

                // 根据会话类型和目标ID获取会话
                // 注意：在实际实现中，可能需要先检查会话是否存在，或者查找会话的ID
                // 这里简化为直接根据会话类型和目标ID标记消息为已读
                
                // 获取会话实体
                var conversationId = targetId; // 这里简化处理，实际中可能需要查询数据库获取会话ID

                // 标记会话所有消息为已读
                var result = await _messageRepository.MarkConversationAsReadAsync(userId, conversationId);

                if (result)
                {
                    _logger.LogInformation($"用户 {userId} 标记与 {conversationType} {targetId} 的所有消息为已读");
                }
                else
                {
                    _logger.LogWarning($"用户 {userId} 标记会话所有消息为已读失败");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"标记会话所有消息为已读失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 撤回消息
        /// </summary>
        public async Task<(bool Success, string Message)> RecallMessageAsync(Guid messageId, Guid userId)
        {
            try
            {
                // 验证参数
                if (messageId == Guid.Empty)
                    throw new ArgumentException("消息ID不能为空", nameof(messageId));
                if (userId == Guid.Empty)
                    throw new ArgumentException("用户ID不能为空", nameof(userId));

                // 获取消息详情，验证是否为发送者
                var message = await _messageRepository.GetByIdAsync(messageId);
                if (message == null)
                {
                    return (false, "消息不存在");
                }

                // 检查是否为消息发送者
                if (message.SenderId != userId)
                {
                    return (false, "只能撤回自己发送的消息");
                }

                // 检查消息发送时间是否在允许撤回的时间范围内（2分钟内）
                var messageAge = DateTime.UtcNow - message.SendTime;
                if (messageAge.TotalMinutes > 2)
                {
                    return (false, "消息发送超过2分钟，无法撤回");
                }

                // 撤回消息
                var result = await _messageRepository.RecallMessageAsync(messageId, userId);

                if (result)
                {
                    _logger.LogInformation($"用户 {userId} 撤回消息 {messageId} 成功");
                    return (true, "消息撤回成功");
                }
                else
                {
                    _logger.LogWarning($"用户 {userId} 撤回消息 {messageId} 失败");
                    return (false, "消息撤回失败");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"撤回消息失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取用户未读消息数
        /// </summary>
        public async Task<int> GetUserUnreadMessageCountAsync(Guid userId)
        {
            try
            {
                // 验证参数
                if (userId == Guid.Empty)
                    throw new ArgumentException("用户ID不能为空", nameof(userId));

                // 获取未读消息数
                var count = await _messageRepository.GetUserUnreadMessageCountAsync(userId);
                
                _logger.LogInformation($"用户 {userId} 有 {count} 条未读消息");
                
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取用户未读消息数失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取会话未读消息数
        /// </summary>
        public async Task<int> GetConversationUnreadMessageCountAsync(Guid userId, ConversationType conversationType, Guid targetId)
        {
            try
            {
                // 验证参数
                if (userId == Guid.Empty)
                    throw new ArgumentException("用户ID不能为空", nameof(userId));
                if (targetId == Guid.Empty)
                    throw new ArgumentException("目标ID不能为空", nameof(targetId));

                // 获取会话实体
                var conversationId = targetId; // 这里简化处理，实际中可能需要查询数据库获取会话ID

                // 获取会话未读消息数
                var count = await _messageRepository.GetConversationUnreadMessageCountAsync(userId, conversationId);
                
                _logger.LogInformation($"用户 {userId} 在与 {conversationType} {targetId} 的会话中有 {count} 条未读消息");
                
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取会话未读消息数失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取消息详情
        /// </summary>
        public async Task<MessageDto> GetMessageByIdAsync(Guid messageId)
        {
            try
            {
                // 验证参数
                if (messageId == Guid.Empty)
                    throw new ArgumentException("消息ID不能为空", nameof(messageId));

                // 获取消息详情
                var message = await _messageRepository.GetByIdAsync(messageId, 
                    m => m.Sender, 
                    m => m.Recipient, 
                    m => m.Group, 
                    m => m.ReplyToMessage,
                    m => m.ReadReceipts);

                if (message == null)
                {
                    return null;
                }

                // 转换为DTO
                var messageDto = MapMessageToDto(message);
                
                return messageDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取消息详情失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取消息已读状态
        /// </summary>
        public async Task<IEnumerable<MessageReadReceiptDto>> GetMessageReadStatusAsync(Guid messageId)
        {
            try
            {
                // 验证参数
                if (messageId == Guid.Empty)
                    throw new ArgumentException("消息ID不能为空", nameof(messageId));

                // 获取消息已读状态
                var readReceipts = await _messageRepository.GetMessageReadStatusAsync(messageId);

                // 转换为DTO
                var readReceiptDtos = readReceipts.Select(r => new MessageReadReceiptDto
                {
                    UserId = r.ReadByUserId,
                    UserName = r.ReadByUser?.Username,
                    ReadAt = r.ReadAt
                }).ToList();
                
                return readReceiptDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取消息已读状态失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 转发消息
        /// </summary>
        public async Task<Message> ForwardMessageAsync(Guid messageId, Guid senderId, ConversationType conversationType, Guid targetId)
        {
            try
            {
                // 验证参数
                if (messageId == Guid.Empty)
                    throw new ArgumentException("消息ID不能为空", nameof(messageId));
                if (senderId == Guid.Empty)
                    throw new ArgumentException("发送者ID不能为空", nameof(senderId));
                if (targetId == Guid.Empty)
                    throw new ArgumentException("目标ID不能为空", nameof(targetId));

                // 获取原始消息
                var originalMessage = await _messageRepository.GetByIdAsync(messageId);
                if (originalMessage == null)
                {
                    throw new InvalidOperationException("要转发的消息不存在");
                }

                // 创建新消息（转发消息）
                Message newMessage;

                // 根据会话类型创建不同类型的消息
                if (conversationType == ConversationType.Private)
                {
                    // 私聊消息
                    newMessage = await SendPrivateMessageAsync(
                        senderId,
                        targetId,
                        originalMessage.Type,
                        originalMessage.Content,
                        null, // 转发消息不设置回复消息ID
                        null,
                        originalMessage.ExtendedData);
                }
                else if (conversationType == ConversationType.Group)
                {
                    // 群聊消息
                    newMessage = await SendGroupMessageAsync(
                        senderId,
                        targetId,
                        originalMessage.Type,
                        originalMessage.Content,
                        null, // 转发消息不设置回复消息ID
                        null,
                        originalMessage.ExtendedData);
                }
                else
                {
                    throw new InvalidOperationException($"不支持的会话类型: {conversationType}");
                }

                _logger.LogInformation($"用户 {senderId} 转发消息 {messageId} 到 {conversationType} {targetId} 成功，新消息ID: {newMessage.Id}");
                
                return newMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"转发消息失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取用户离线消息
        /// </summary>
        public async Task<IEnumerable<MessageDto>> GetOfflineMessagesAsync(Guid userId, DateTime lastSyncTimestamp)
        {
            try
            {
                // 验证参数
                if (userId == Guid.Empty)
                    throw new ArgumentException("用户ID不能为空", nameof(userId));

                // 获取指定时间后的新消息
                var messages = await _messageRepository.GetMessagesAfterTimestampAsync(userId, lastSyncTimestamp);

                // 转换为DTO
                var messageDtos = messages.Select(MapMessageToDto).ToList();
                
                _logger.LogInformation($"用户 {userId} 获取到 {messageDtos.Count} 条离线消息");
                
                return messageDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取用户离线消息失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 删除消息（仅针对自己）
        /// </summary>
        public async Task<bool> DeleteMessageForUserAsync(Guid messageId, Guid userId)
        {
            // 注意：这里实现的是逻辑删除，只对当前用户不可见，但消息仍然存在于数据库中
            // 实际实现可能需要更复杂的逻辑，例如添加用户删除消息的记录
            
            try
            {
                // 验证参数
                if (messageId == Guid.Empty)
                    throw new ArgumentException("消息ID不能为空", nameof(messageId));
                if (userId == Guid.Empty)
                    throw new ArgumentException("用户ID不能为空", nameof(userId));

                // 获取消息详情
                var message = await _messageRepository.GetByIdAsync(messageId);
                if (message == null)
                {
                    return false;
                }

                // TODO: 实现消息删除逻辑
                // 目前的领域模型中没有为特定用户删除消息的实现
                // 可能需要添加UserDeletedMessage表来记录用户删除的消息

                _logger.LogWarning($"删除消息功能尚未完全实现");
                
                // 临时实现：返回成功
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除消息失败: {ex.Message}");
                throw;
            }
        }

        #region 辅助方法

        /// <summary>
        /// 将消息实体转换为DTO
        /// </summary>
        private MessageDto MapMessageToDto(Message message)
        {
            if (message == null)
                return null;

            var dto = new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderName = message.Sender?.Username ?? "未知用户",
                SenderAvatar = message.Sender?.AvatarUrl,
                RecipientId = message.RecipientId,
                GroupId = message.GroupId,
                Type = message.Type.ToString(),
                Content = message.Content,
                ReplyToMessageId = message.ReplyToMessageId,
                Status = message.Status.ToString(),
                SendTime = message.SendTime,
                DeliveredTime = message.DeliveredTime,
                ReadTime = message.ReadTime,
                ExtendedData = message.ExtendedData
            };

            // 如果有引用回复的消息，添加引用消息信息
            if (message.ReplyToMessage != null)
            {
                dto.ReplyToMessage = new MessageReferenceDto
                {
                    Id = message.ReplyToMessage.Id,
                    SenderId = message.ReplyToMessage.SenderId,
                    SenderName = message.ReplyToMessage.Sender?.Username ?? "未知用户",
                    Type = message.ReplyToMessage.Type.ToString(),
                    ContentPreview = message.ReplyToMessage.Content
                };
            }

            // 添加已读回执信息
            if (message.ReadReceipts != null && message.ReadReceipts.Any())
            {
                dto.ReadReceipts = message.ReadReceipts.Select(r => new MessageReadReceiptDto
                {
                    UserId = r.ReadByUserId,
                    UserName = r.ReadByUser?.Username,
                    ReadAt = r.ReadAt
                }).ToList();
            }
            else
            {
                dto.ReadReceipts = new List<MessageReadReceiptDto>();
            }

            return dto;
        }

        #endregion
    }
}