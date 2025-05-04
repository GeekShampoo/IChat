using IChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// 通用仓储接口，定义所有实体共用的基本操作
    /// </summary>
    /// <typeparam name="T">实体类型，必须继承自 BaseEntity</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        // 查询操作
        
        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <param name="includeProperties">需要进行预加载的属性表达式数组</param>
        /// <returns>实体集合</returns>
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
        
        /// <summary>
        /// 根据条件查询实体
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="includeProperties">需要进行预加载的属性表达式数组</param>
        /// <returns>符合条件的实体集合</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties);
        
        /// <summary>
        /// 根据 ID 获取单个实体
        /// </summary>
        /// <param name="id">实体 ID</param>
        /// <param name="includeProperties">需要进行预加载的属性表达式数组</param>
        /// <returns>实体对象，未找到则返回 null</returns>
        Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties);
        
        /// <summary>
        /// 根据条件获取单个实体
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="includeProperties">需要进行预加载的属性表达式数组</param>
        /// <returns>实体对象，未找到则返回 null</returns>
        Task<T> GetSingleAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties);
        
        /// <summary>
        /// 分页查询实体
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="pageIndex">页码，从 1 开始</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="includeProperties">需要进行预加载的属性表达式数组</param>
        /// <returns>分页后的实体集合</returns>
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            Expression<Func<T, bool>> filter, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, 
            int pageIndex, 
            int pageSize, 
            params Expression<Func<T, object>>[] includeProperties);
        
        /// <summary>
        /// 判断是否存在符合条件的实体
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <returns>如果存在符合条件的实体，则返回 true；否则返回 false</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
        
        /// <summary>
        /// 获取符合条件的实体数量
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <returns>符合条件的实体数量</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> filter = null);
        
        // 修改操作
        
        /// <summary>
        /// 添加单个实体
        /// </summary>
        /// <param name="entity">要添加的实体</param>
        /// <returns>添加后的实体</returns>
        Task<T> AddAsync(T entity);
        
        /// <summary>
        /// 添加多个实体
        /// </summary>
        /// <param name="entities">要添加的实体集合</param>
        /// <returns>添加后的实体集合</returns>
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        
        /// <summary>
        /// 更新单个实体
        /// </summary>
        /// <param name="entity">要更新的实体</param>
        /// <returns>更新后的实体</returns>
        Task<T> UpdateAsync(T entity);
        
        /// <summary>
        /// 删除单个实体
        /// </summary>
        /// <param name="entity">要删除的实体</param>
        /// <returns>操作是否成功</returns>
        Task<bool> DeleteAsync(T entity);
        
        /// <summary>
        /// 根据 ID 删除单个实体
        /// </summary>
        /// <param name="id">要删除的实体 ID</param>
        /// <returns>操作是否成功</returns>
        Task<bool> DeleteByIdAsync(Guid id);
        
        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <param name="entities">要删除的实体集合</param>
        /// <returns>操作是否成功</returns>
        Task<bool> DeleteRangeAsync(IEnumerable<T> entities);
        
        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <returns>删除的实体数量</returns>
        Task<int> DeleteWhereAsync(Expression<Func<T, bool>> filter);
    }
}