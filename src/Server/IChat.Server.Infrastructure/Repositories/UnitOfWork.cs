using IChat.Domain.Entities;
using IChat.Domain.Interfaces;
using IChat.Server.Infrastructure.Data;
using IChat.Server.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IChat.Server.Infrastructure.Repositories
{
    /// <summary>
    /// 工作单元的实现，管理仓储实例和事务
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IChatDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction _transaction;
        private bool _disposed;

        public UnitOfWork(IChatDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _repositories = new Dictionary<Type, object>();
        }

        public IRepository<T> Repository<T>() where T : BaseEntity
        {
            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                var repository = new Repository<T>(_dbContext);
                _repositories.Add(type, repository);
            }

            return (IRepository<T>)_repositories[type];
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void BeginTransaction()
        {
            // 注意：直接使用 BeginTransaction 方法与重试策略不兼容
            // 应该使用 ExecuteInTransactionAsync 方法
            _transaction = _dbContext.Database.BeginTransaction();
        }

        public async Task BeginTransactionAsync()
        {
            // 注意：直接使用 BeginTransactionAsync 方法与重试策略不兼容
            // 应该使用 ExecuteInTransactionAsync 方法
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public void CommitTransaction()
        {
            try
            {
                _dbContext.SaveChanges();
                _transaction?.Commit();
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _transaction?.Rollback();
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        /// <summary>
        /// 在事务中执行操作，支持自动重试
        /// </summary>
        /// <typeparam name="TResult">操作结果类型</typeparam>
        /// <param name="action">要执行的操作</param>
        /// <returns>操作结果</returns>
        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action)
        {
            // 获取执行策略
            var strategy = _dbContext.Database.CreateExecutionStrategy();

            // 在执行策略内执行事务操作
            return await strategy.ExecuteAsync(async () =>
            {
                // 开始事务
                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    // 执行业务逻辑
                    var result = await action();

                    // 提交事务
                    await transaction.CommitAsync();

                    return result;
                }
                catch
                {
                    // 事务会自动回滚
                    throw;
                }
            });
        }

        /// <summary>
        /// 在事务中执行操作，支持自动重试
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <returns>任务</returns>
        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            // 获取执行策略
            var strategy = _dbContext.Database.CreateExecutionStrategy();

            // 在执行策略内执行事务操作
            await strategy.ExecuteAsync(async () =>
            {
                // 开始事务
                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    // 执行业务逻辑
                    await action();

                    // 提交事务
                    await transaction.CommitAsync();
                }
                catch
                {
                    // 事务会自动回滚
                    throw;
                }
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _transaction?.Dispose();
                _dbContext.Dispose();
                _disposed = true;
            }
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}