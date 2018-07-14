using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Framework.Common;
using Framework.Model;
using Framework.Model.Repository;
using Microsoft.EntityFrameworkCore;

namespace Framework.Data.Ef
{
    public abstract class EfRepository<TEntity, TContext, TId> : IRepository<TEntity, TId>, IReadOnlyRepository<TEntity, TId> where TEntity : class, IEntity<TId> where TContext : DbContext where TId : IEquatable<TId>
    {
        protected readonly DbSet<TEntity> Entities;
        protected EfRepository(TContext dbContext)
        {
            Entities = dbContext.Set<TEntity>();
        }

        public async Task<IEnumerable<TResult>> GetBy<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken), params Expression<Func<TEntity, object>>[] includes)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var queryable = Entities.AsNoTracking();
            if (includes != null)
                queryable = includes.Aggregate(queryable, (current, include) => current.Include(include));
            return await queryable.Where(filter).Select(selector).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task<PaginatedList<TResult>> GetPagedItems<TResult>(SortDirection sort, string sortBy, Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> selector, int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default(CancellationToken), params Expression<Func<TEntity, object>>[] includes)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var queryable = Entities.AsNoTracking();
            if (includes != null)
                queryable = includes.Aggregate(queryable, (current, include) => current.Include(include));
            return SortBy(queryable.Where(filter), sort, sortBy).CreatePaginatedList<TEntity, TId, TResult>(selector, pageIndex, pageSize, cancellationToken);
        }

        public Task<TResult> GetSingleBy<TResult>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken), params Expression<Func<TEntity, object>>[] includes)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var queryable = Entities.AsNoTracking();
            if (includes != null)
                queryable = includes.Aggregate(queryable, (current, include) => current.Include(include));
            return queryable.Where(filter).Select(selector).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<TResult>> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken), params Expression<Func<TEntity, object>>[] includes)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var queryable = Entities.AsNoTracking();
            if (includes != null)
                queryable = includes.Aggregate(queryable, (current, include) => current.Include(include));
            return await queryable.Select(selector).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task<bool> All(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Entities.AsNoTracking().AllAsync(filter, cancellationToken);
        }

        public Task<bool> Any(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Entities.AsNoTracking().AnyAsync(filter, cancellationToken);
        }

        public Task Add(TEntity item, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            return Entities.AddAsync(item, cancellationToken);
        }

        public Task Delete(TEntity item, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Entities.Remove(item);
            return Task.CompletedTask;
        }

        public Task DeleteRange(IEnumerable<TEntity> items, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            Entities.RemoveRange(items);
            return Task.CompletedTask;
        }

        public Task AddRange(IEnumerable<TEntity> items, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            return Entities.AddRangeAsync(items, cancellationToken);
        }

        public Task Edit(TEntity item, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            Entities.Attach(item);
            if (item is IHasConcurrencyStamp)
                ((IHasConcurrencyStamp)item).ConcurrencyStamp = Guid.NewGuid().ToString();
            Entities.Update(item);
            return Task.CompletedTask;
        }

        protected virtual IQueryable<TEntity> SortBy(IQueryable<TEntity> source, SortDirection sort, string sortBy)
        {
            return sort != SortDirection.Ascending ? source.OrderByDescending(c => c.Id) : source.OrderBy(c => c.Id);
        }
    }
}