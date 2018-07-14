using System;
using Framework.Data.Ef;
using Framework.Model;

namespace SampleApi.V1.Data
{
    public class Repository<TEntity, TId> : EfRepository<TEntity,SampleDbContext,TId> where TEntity : class, IEntity<TId> where TId : IEquatable<TId>
    {
        public Repository(SampleDbContext dbContext) : base(dbContext)
        {
        }
    }
}