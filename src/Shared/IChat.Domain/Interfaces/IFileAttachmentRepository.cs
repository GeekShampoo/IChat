using IChat.Domain.Entities;
using IChat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// FileAttachment 实体的仓储接口，继承自通用仓储接口，添加特定于文件附件的操作
    /// </summary>
    public interface IFileAttachmentRepository : IRepository<FileAttachment>
    {
        /// <summary>
        /// 获取用户上传的文件
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pageIndex">页码，从 1 开始</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>分页后的文件列表和总数</returns>
        Task<(IEnumerable<FileAttachment> Items, int TotalCount)> GetUserFilesAsync(Guid userId, int pageIndex, int pageSize);
        
        /// <summary>
        /// 获取与特定消息关联的文件
        /// </summary>
        /// <param name="messageId">消息ID</param>
        /// <returns>文件列表</returns>
        Task<IEnumerable<FileAttachment>> GetMessageFilesAsync(Guid messageId);
        
        /// <summary>
        /// 根据哈希值获取文件
        /// </summary>
        /// <param name="fileHash">文件哈希值</param>
        /// <returns>文件，不存在则返回 null</returns>
        Task<FileAttachment> GetFileByHashAsync(string fileHash);
        
        /// <summary>
        /// 根据文件类型获取文件
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="fileType">文件类型</param>
        /// <param name="pageIndex">页码，从 1 开始</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>分页后的文件列表和总数</returns>
        Task<(IEnumerable<FileAttachment> Items, int TotalCount)> GetFilesByTypeAsync(Guid userId, string fileType, int pageIndex, int pageSize);
        
        /// <summary>
        /// 更新文件状态
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="status">新状态</param>
        /// <returns>更新后的文件</returns>
        Task<FileAttachment> UpdateFileStatusAsync(Guid fileId, FileStatus status);
        
        /// <summary>
        /// 关联文件到消息
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <param name="messageId">消息ID</param>
        /// <returns>操作是否成功</returns>
        Task<bool> AssociateFileWithMessageAsync(Guid fileId, Guid messageId);
        
        /// <summary>
        /// 获取文件的大小总和
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>文件大小总和（字节）</returns>
        Task<long> GetUserTotalFilesSizeAsync(Guid userId);
        
        /// <summary>
        /// 获取最近上传的文件
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="count">要获取的数量</param>
        /// <returns>文件列表</returns>
        Task<IEnumerable<FileAttachment>> GetRecentFilesAsync(Guid userId, int count);
    }
}