using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Framework.Common;
using Framework.Model;
using Microsoft.EntityFrameworkCore;

namespace Framework.Data.Ef {
    internal static class Extensions {
        internal static async Task<PaginatedList<TResult>> CreatePaginatedList<TEntity, TId, TResult>(
            this IQueryable<TEntity> source, Expression<Func<TEntity, TResult>> selector, int pageIndex, int pageSize,
            CancellationToken cancellationToken = default(CancellationToken))
            where TEntity : IEntity<TId> where TId : IEquatable<TId> {
            var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
            return new PaginatedList<TResult>(
                await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(selector)
                    .ToListAsync(cancellationToken).ConfigureAwait(false), count, pageIndex, pageSize);
        }
    }
}