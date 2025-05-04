using System;
using IChat.Domain.Enums;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 文件附件实体，表示消息中包含的文件
    /// </summary>
    public class FileAttachment : BaseEntity
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 存储路径
        /// </summary>
        public string StoragePath { get; set; }

        /// <summary>
        /// 访问URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 缩略图URL（如果是图片、视频等）
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// 上传者ID
        /// </summary>
        public Guid UploaderId { get; set; }

        /// <summary>
        /// 关联的消息ID
        /// </summary>
        public Guid? MessageId { get; set; }

        /// <summary>
        /// 文件状态
        /// </summary>
        public FileStatus Status { get; set; } = FileStatus.Uploading;

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadedAt { get; set; }

        /// <summary>
        /// 过期时间（如果有）
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// MD5校验值
        /// </summary>
        public string Md5Hash { get; set; }

        /// <summary>
        /// 文件上传者
        /// </summary>
        public virtual User Uploader { get; set; }

        /// <summary>
        /// 关联的消息
        /// </summary>
        public virtual Message Message { get; set; }
    }
}