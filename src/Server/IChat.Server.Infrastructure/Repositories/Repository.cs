using IChat.Domain.Entities;
using IChat.Domain.Interfaces;
using IChat.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IChat.Server.Infrastructure.Repositories
{
    /// <summary>
    /// 通用仓储接口的实现类
    /// </summary>
    /// <typeparam name="T">实体类型，必须继承自BaseEntity</typeparam>
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly IChatDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public Repository(IChatDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<T>();
        }

        #region 查询操作

        public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            
            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            
            if (filter != null)
            {
                query = query.Where(filter);
            }
            
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            
            return await query.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            
            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            
            if (filter != null)
            {
                query = query.Where(filter);
            }
            
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            
            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            Expression<Func<T, bool>> filter, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, 
            int pageIndex, 
            int pageSize, 
            params Expression<Func<T, object>>[] includeProperties)
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            
            if (pageSize < 1)
            {
                pageSize = 10;
            }
            
            IQueryable<T> query = _dbSet;
            
            if (filter != null)
            {
                query = query.Where(filter);
            }
            
            // 计算总记录数
            int totalCount = await query.CountAsync();
            
            // 应用包含属性
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            
            // 应用排序
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            
            // 应用分页
            var pagedItems = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return (pagedItems, totalCount);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }
            
            return await _dbSet.AnyAsync(filter);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet;
            
            if (filter != null)
            {
                query = query.Where(filter);
            }
            
            return await query.CountAsync();
        }

        #endregion

        #region 修改操作

        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            
            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            
            await _dbSet.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
            
            return entities;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            _dbSet.Remove(entity);
            int result = await _dbContext.SaveChangesAsync();
            
            return result > 0;
        }

        public virtual async Task<bool> DeleteByIdAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            
            if (entity == null)
            {
                return false;
            }
            
            _dbSet.Remove(entity);
            int result = await _dbContext.SaveChangesAsync();
            
            return result > 0;
        }

        public virtual async Task<bool> DeleteRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }
            
            _dbSet.RemoveRange(entities);
            int result = await _dbContext.SaveChangesAsync();
            
            return result > 0;
        }

        public virtual async Task<int> DeleteWhereAsync(Expression<Func<T, bool>> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }
            
            var entities = await _dbSet.Where(filter).ToListAsync();
            _dbSet.RemoveRange(entities);
            
            return await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}