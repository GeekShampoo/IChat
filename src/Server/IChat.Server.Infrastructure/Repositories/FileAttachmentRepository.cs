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
    /// FileAttachment实体的仓储实现类
    /// </summary>
    public class FileAttachmentRepository : Repository<FileAttachment>, IFileAttachmentRepository
    {
        public FileAttachmentRepository(IChatDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(IEnumerable<FileAttachment> Items, int TotalCount)> GetUserFilesAsync(Guid userId, int pageIndex, int pageSize)
        {
            IQueryable<FileAttachment> query = _dbSet
                .Where(f => f.UploaderId == userId && !f.IsDeleted);

            int totalCount = await query.CountAsync();

            var pagedItems = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (pagedItems, totalCount);
        }

        public async Task<IEnumerable<FileAttachment>> GetMessageFilesAsync(Guid messageId)
        {
            return await _dbSet
                .Where(f => f.MessageId == messageId && !f.IsDeleted)
                .ToListAsync();
        }

        public async Task<FileAttachment> GetFileByHashAsync(string fileHash)
        {
            if (string.IsNullOrWhiteSpace(fileHash))
            {
                throw new ArgumentException("文件哈希不能为空", nameof(fileHash));
            }

            return await _dbSet
                .FirstOrDefaultAsync(f => f.Md5Hash == fileHash && !f.IsDeleted);
        }

        public async Task<(IEnumerable<FileAttachment> Items, int TotalCount)> GetFilesByTypeAsync(Guid userId, string fileType, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(fileType))
            {
                throw new ArgumentException("文件类型不能为空", nameof(fileType));
            }

            IQueryable<FileAttachment> query = _dbSet
                .Where(f => f.UploaderId == userId && f.FileType == fileType && !f.IsDeleted);

            int totalCount = await query.CountAsync();

            var pagedItems = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (pagedItems, totalCount);
        }

        public async Task<FileAttachment> UpdateFileStatusAsync(Guid fileId, FileStatus status)
        {
            var file = await _dbSet.FindAsync(fileId);
            
            if (file == null)
            {
                throw new ArgumentException($"未找到ID为{fileId}的文件", nameof(fileId));
            }

            file.Status = status;
            file.UpdatedAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
            return file;
        }

        public async Task<bool> AssociateFileWithMessageAsync(Guid fileId, Guid messageId)
        {
            var file = await _dbSet.FindAsync(fileId);
            
            if (file == null)
            {
                throw new ArgumentException($"未找到ID为{fileId}的文件", nameof(fileId));
            }

            file.MessageId = messageId;
            file.UpdatedAt = DateTime.UtcNow;
            
            int result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<long> GetUserTotalFilesSizeAsync(Guid userId)
        {
            return await _dbSet
                .Where(f => f.UploaderId == userId && !f.IsDeleted)
                .SumAsync(f => (long)f.FileSize);
        }

        public async Task<IEnumerable<FileAttachment>> GetRecentFilesAsync(Guid userId, int count)
        {
            return await _dbSet
                .Where(f => f.UploaderId == userId && !f.IsDeleted)
                .OrderByDescending(f => f.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}