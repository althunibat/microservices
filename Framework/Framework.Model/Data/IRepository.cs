using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Model.Data {
    public interface IRepository<in TEntity, TId>
        where TId : IEquatable<TId>
        where TEntity : IEntity<TId> {
        Task Add(TEntity item, CancellationToken cancellationToken = default(CancellationToken));
        Task Edit(TEntity item, CancellationToken cancellationToken = default(CancellationToken));
        Task Delete(TEntity item, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteRange(IEnumerable<TEntity> items, CancellationToken cancellationToken = default(CancellationToken));
        Task AddRange(IEnumerable<TEntity> items, CancellationToken cancellationToken = default(CancellationToken));
    }
    public interface IRepository<in TEntity> : IRepository<TEntity, long> where TEntity : Entity { }

}