using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IUnitOfWork UnitOfWork { get; }
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> AddAsync(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task<TEntity> UpdateAsync(TEntity entity);
        void UpdateRangeAsync(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        Task<TEntity> FindAsyncOptimized(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> FindManyAsyncOptimized(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity> FindAsyncOptimizedByPath(Expression<Func<TEntity, bool>> predicate, params string[] includes);
        Task<IEnumerable<TEntity>> FindManyAsyncOptimizedByPath(Expression<Func<TEntity, bool>> predicate, params string[] includes);
    }
}
