using System;
using IChat.Protocol.Contracts;

namespace IChat.Protocol.Dtos.File
{
    /// <summary>
    /// 文件信息DTO
    /// </summary>
    public class FileDto
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 文件名
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
        /// 文件URL（用于下载）
        /// </summary>
        public string FileUrl { get; set; }

        /// <summary>
        /// 缩略图URL（图片和视频）
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// 上传者ID
        /// </summary>
        public Guid UploaderId { get; set; }

        /// <summary>
        /// 上传者名称
        /// </summary>
        public string UploaderName { get; set; }

        /// <summary>
        /// 文件状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 过期时间（如果有）
        /// </summary>
        public DateTime? ExpiryTime { get; set; }

        /// <summary>
        /// 是否为临时文件
        /// </summary>
        public bool IsTemporary { get; set; }

        /// <summary>
        /// 文件MD5校验码
        /// </summary>
        public string Md5Hash { get; set; }
    }

    /// <summary>
    /// 文件上传初始化请求
    /// </summary>
    public class InitializeUploadRequest : BaseRequest
    {
        /// <summary>
        /// 文件名
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
        /// 文件MD5校验码
        /// </summary>
        public string Md5Hash { get; set; }

        /// <summary>
        /// 是否为临时文件（临时文件将在一段时间后自动删除）
        /// </summary>
        public bool IsTemporary { get; set; } = false;
    }

    /// <summary>
    /// 文件上传初始化响应
    /// </summary>
    public class InitializeUploadResponse : BaseResponse<InitializeUploadResultDto>
    {
    }

    /// <summary>
    /// 文件上传初始化结果DTO
    /// </summary>
    public class InitializeUploadResultDto
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// 上传URL（用于直接上传文件）
        /// </summary>
        public string UploadUrl { get; set; }

        /// <summary>
        /// 是否已存在相同文件（相同MD5）
        /// </summary>
        public bool FileExists { get; set; }

        /// <summary>
        /// 如果文件已存在，则提供文件信息
        /// </summary>
        public FileDto ExistingFile { get; set; }

        /// <summary>
        /// 上传令牌（用于后续请求）
        /// </summary>
        public string UploadToken { get; set; }

        /// <summary>
        /// 分片大小（字节）
        /// </summary>
        public int ChunkSize { get; set; }

        /// <summary>
        /// 是否需要分片上传
        /// </summary>
        public bool RequireChunking { get; set; }

        /// <summary>
        /// 上传过期时间
        /// </summary>
        public DateTime ExpiryTime { get; set; }
    }

    /// <summary>
    /// 文件分片上传请求
    /// </summary>
    public class UploadChunkRequest : BaseRequest
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// 上传令牌
        /// </summary>
        public string UploadToken { get; set; }

        /// <summary>
        /// 分片索引
        /// </summary>
        public int ChunkIndex { get; set; }

        /// <summary>
        /// 总分片数
        /// </summary>
        public int TotalChunks { get; set; }

        /// <summary>
        /// 当前分片MD5校验码
        /// </summary>
        public string ChunkMd5Hash { get; set; }

        // 注意：实际文件数据会通过multipart/form-data方式上传，不包含在此DTO中
    }

    /// <summary>
    /// 完成文件上传请求
    /// </summary>
    public class CompleteUploadRequest : BaseRequest
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// 上传令牌
        /// </summary>
        public string UploadToken { get; set; }

        /// <summary>
        /// 总分片数（验证）
        /// </summary>
        public int TotalChunks { get; set; }

        /// <summary>
        /// 文件MD5校验码（验证）
        /// </summary>
        public string Md5Hash { get; set; }
    }

    /// <summary>
    /// 完成文件上传响应
    /// </summary>
    public class CompleteUploadResponse : BaseResponse<FileDto>
    {
    }

    /// <summary>
    /// 下载文件请求
    /// </summary>
    public class DownloadFileRequest : BaseRequest
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid FileId { get; set; }
    }

    /// <summary>
    /// 下载文件响应
    /// </summary>
    public class DownloadFileResponse : BaseResponse<DownloadFileResultDto>
    {
    }

    /// <summary>
    /// 下载文件结果DTO
    /// </summary>
    public class DownloadFileResultDto
    {
        /// <summary>
        /// 文件信息
        /// </summary>
        public FileDto FileInfo { get; set; }

        /// <summary>
        /// 下载URL
        /// </summary>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// 下载URL过期时间
        /// </summary>
        public DateTime UrlExpiryTime { get; set; }
    }

    /// <summary>
    /// 删除文件请求
    /// </summary>
    public class DeleteFileRequest : BaseRequest
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public Guid FileId { get; set; }
    }
}