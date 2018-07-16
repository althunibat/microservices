using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Framework.Common;

namespace Framework.Model.Repository {
    public interface IReadOnlyRepository<TEntity, TId> where TEntity : IEntity<TId> where TId : IEquatable<TId> {
        Task<PaginatedList<TResult>> GetPagedItems<TResult>(SortDirection sort, string sortBy,
            Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TResult>> selector, int pageIndex = 1,
            int pageSize = 10, CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TResult>> GetBy<TResult>(Expression<Func<TEntity, bool>> filter,
            Expression<Func<TEntity, TResult>> selector,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TResult>> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<TResult> GetSingleBy<TResult>(Expression<Func<TEntity, bool>> filter,
            Expression<Func<TEntity, TResult>> selector,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<TEntity, object>>[] includes);

        Task<bool> All(Expression<Func<TEntity, bool>> filter,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> Any(Expression<Func<TEntity, bool>> filter,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}