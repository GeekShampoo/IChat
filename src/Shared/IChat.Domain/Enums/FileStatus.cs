namespace IChat.Domain.Enums
{
    /// <summary>
    /// 文件状态枚举
    /// </summary>
    public enum FileStatus
    {
        /// <summary>
        /// 上传中
        /// </summary>
        Uploading = 0,
        
        /// <summary>
        /// 上传完成
        /// </summary>
        Uploaded = 1,
        
        /// <summary>
        /// 处理中（如生成缩略图等）
        /// </summary>
        Processing = 2,
        
        /// <summary>
        /// 可用状态
        /// </summary>
        Available = 3,
        
        /// <summary>
        /// 上传失败
        /// </summary>
        Failed = 4,
        
        /// <summary>
        /// 已过期
        /// </summary>
        Expired = 5,
        
        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = 6
    }
}