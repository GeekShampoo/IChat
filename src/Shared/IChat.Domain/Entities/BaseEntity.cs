using System;

namespace IChat.Domain.Entities
{
    /// <summary>
    /// 所有领域实体的基类，包含通用属性
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// 实体的唯一标识符
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 是否已删除（软删除标记）
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}