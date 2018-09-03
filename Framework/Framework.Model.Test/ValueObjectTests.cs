using System;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Framework.Model.Test {
    public class ValueObjectTests {

        [Fact]
        public void TwoObjectsWithSameAtomicValuesShouldBeEqual() {
            var obj1 = new MockValueObject(1, 2, "test");
            var obj2 = new MockValueObject(1, 2, "test");
            obj2.Should().Be(obj1);
        }

        [Fact]
        public void TwoObjectsWithSameAtomicValuesShouldHaveEqualHashCodes() {
            var hc1 = new MockValueObject(1, 2, "test").GetHashCode();
            var hc2 = new MockValueObject(1, 2, "test").GetHashCode();
            hc1.Should().Be(hc2);
        }

        [Fact]
        public void CopyInstanceOfValueObjectShouldBeEqual() {
            var obj = new MockValueObject(1,2,"test");
            var cpy = obj.GetCopy();
            obj.Should().Be(cpy);
        }

        [Fact]
        public void TwoObjectsWithDifferentAtomicValuesShouldBeEqual() {
            var obj1 = new MockValueObject(1, 2, "test");
            var obj2 = new MockValueObject(1, 2);
            obj2.Should().NotBe(obj1);
        }

        public class MockValueObject : ValueObject<MockValueObject> {
            private readonly List<object> _list;
            public MockValueObject(params object[] args) {
                _list = args == null ? new List<object>() : new List<object>(args);
            }

            protected override IEnumerable<object> GetAtomicValues() {
                return _list;
            }
        }
    }
}
