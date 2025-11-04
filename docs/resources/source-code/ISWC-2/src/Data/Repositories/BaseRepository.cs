using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

// TODO: replace this with Autofac module registration
[assembly: InternalsVisibleTo("SpanishPoint.Azure.Iswc.Jobs")]
namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    internal abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly CsiContext context;
        private readonly CancellationToken? cancellationToken;

        public IUnitOfWork UnitOfWork => context;

        public BaseRepository() { }
        public BaseRepository(CsiContext context, IHttpContextAccessor httpContext)
        {
            this.context = context;
            cancellationToken = httpContext.HttpContext?.RequestAborted;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            cancellationToken.ThrowIfCancelled();
            var result = (await context.AddAsync(entity));
            return result.Entity;
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate) => 
            await FindOptimized(predicate).AnyAsync();

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate) =>
            (await FindManyAsync(predicate)).FirstOrDefault();

        public async Task<IEnumerable<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> predicate) =>
            await Task.FromResult(FindOptimized(predicate));

        public async Task<TEntity> FindAsyncOptimized(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes) =>
         (await FindManyAsyncOptimized(predicate, includes)).FirstOrDefault();

        public async Task<IEnumerable<TEntity>> FindManyAsyncOptimized(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes) =>
            await Task.FromResult(FindOptimizedByFunc(predicate, includes));

        public async Task<TEntity> FindAsyncOptimizedByPath(Expression<Func<TEntity, bool>> predicate, params string[] includes) =>
           (await FindManyAsyncOptimizedByPath(predicate, includes)).FirstOrDefault();

        public async Task<IEnumerable<TEntity>> FindManyAsyncOptimizedByPath(Expression<Func<TEntity, bool>> predicate, params string[] includes) =>
            await Task.FromResult(FindOptimizedByPath(predicate, includes));

        private IQueryable<TEntity> FindOptimizedByFunc(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes) =>
            FindOptimized(predicate).IncludeAll(includes);

        private IQueryable<TEntity> FindOptimizedByPath(Expression<Func<TEntity, bool>> predicate, params string[] includes) =>
            FindOptimized(predicate).IncludeAllByPath(includes);

        private IQueryable<TEntity> FindOptimized(Expression<Func<TEntity, bool>> predicate)
        {
            cancellationToken.ThrowIfCancelled();
            return context.Set<TEntity>().AsQueryable().Where(predicate);
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            cancellationToken.ThrowIfCancelled();
            return Task.FromResult(context.Update(entity).Entity);
        }

        public void Delete(TEntity entity)
        {
            cancellationToken.ThrowIfCancelled();
            context.Set<TEntity>().Attach(entity);
            context.Set<TEntity>().Remove(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            cancellationToken.ThrowIfCancelled();
            context.AddRange(entities);
        }
        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            cancellationToken.ThrowIfCancelled();
            await context.AddRangeAsync(entities);
        }

        public void UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            cancellationToken.ThrowIfCancelled();
            context.UpdateRange(entities);
        }
    }
}
