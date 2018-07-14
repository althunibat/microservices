using System;

namespace Framework.Model
{
    public interface IEntity<TId> : IEquatable<IEntity<TId>> where TId : IEquatable<TId>
    {
        TId Id { get; }
    }

   
}