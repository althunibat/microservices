using System;
using System.Collections.Generic;

namespace Framework.Model
{
    public abstract class Entity<TId>:IEntity<TId> where TId: IEquatable<TId>
    {
        protected Entity(TId id)
        {
            if (Equals(id, default(TId)))
                throw new ArgumentOutOfRangeException(nameof(id));
            Id = id;
        }

        protected Entity()
        {
        }

        public TId Id { get; protected set; }

        public bool Equals(IEntity<TId> other)
        {
            return other != null && EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((IEntity<TId>) obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TId>.Default.GetHashCode(Id);
        }

        public static bool operator ==(Entity<TId> left, Entity<TId> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity<TId> left, Entity<TId> right)
        {
            return !Equals(left, right);
        }
    }
    
    public abstract class Entity : Entity<long>
    {
        protected Entity(long id) : base(id)
        {
        }

        protected Entity()
        {
        }
    }
}