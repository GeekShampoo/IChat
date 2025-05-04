using IChat.Domain.Entities;
using IChat.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace IChat.Domain.Interfaces
{
    /// <summary>
    /// 工作单元接口，管理仓储实例和事务
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 获取指定实体的仓储
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>实体仓储</returns>
        IRepository<T> Repository<T>() where T : BaseEntity;

        /// <summary>
        /// 保存所有更改
        /// </summary>
        /// <returns>受影响的行数</returns>
        int SaveChanges();

        /// <summary>
        /// 异步保存所有更改
        /// </summary>
        /// <returns>受影响的行数</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// 开始事务
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// 异步开始事务, 支持执行策略重试
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// 在事务中执行操作，支持自动重试
        /// </summary>
        /// <typeparam name="TResult">操作结果类型</typeparam>
        /// <param name="action">要执行的操作</param>
        /// <returns>操作结果</returns>
        Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action);

        /// <summary>
        /// 在事务中执行操作，支持自动重试
        /// </summary>
        /// <param name="action">要执行的操作</param>
        /// <returns>任务</returns>
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}