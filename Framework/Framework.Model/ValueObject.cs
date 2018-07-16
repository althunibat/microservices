using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Model {
    public abstract class ValueObject<T> : IEquatable<ValueObject<T>> where T : ValueObject<T> {
        public bool Equals(ValueObject<T> other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            var thisValues = GetAtomicValues().GetEnumerator();
            var otherValues = other.GetAtomicValues().GetEnumerator();
            try {
                while (thisValues.MoveNext() && otherValues.MoveNext()) {
                    if (thisValues.Current is null ^ otherValues.Current is null) return false;

                    if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current)) return false;
                }

                return !thisValues.MoveNext() && !otherValues.MoveNext();
            }
            finally {
                thisValues.Dispose();
                otherValues.Dispose();
            }
        }

        protected abstract IEnumerable<object> GetAtomicValues();

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType()) return false;
            var other = (ValueObject<T>) obj;
            return Equals(other);
        }

        public override int GetHashCode() {
            return GetAtomicValues()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        public static bool operator ==(ValueObject<T> left, ValueObject<T> right) {
            return Equals(left, right);
        }

        public static bool operator !=(ValueObject<T> left, ValueObject<T> right) {
            return !Equals(left, right);
        }

        public ValueObject<T> GetCopy() {
            return MemberwiseClone() as ValueObject<T>;
        }
    }
}