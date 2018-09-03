using FluentAssertions;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Framework.Model.Test {
    public class EntityTest {
        [Fact]
        public void InitializeEntityWithDefaultIdValueShouldThrowArgumentOutOfRangeException() {
            Action ac = () => new MockEntity(default(long));
            ac.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void InitializeEntityWithNegativeIdValueShouldThrowArgumentOutOfRangeException() {
            Action ac = () => new MockEntity(-2);
            ac.Should().ThrowExactly<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void InitializeEntityWithPositiveIdValueShouldSetIdValue() {
            var entity = new MockEntity(2);
            entity.Id.Should().Be(2);
        }
        [Fact]
        public void TwoEntityObjectsWithSameIdValueShouldBeEqual() {
            var entity1 = new MockEntity(2);
            var entity2 = new MockEntity(2);
            entity1.Should().Be(entity2);
        }
        [Fact]
        public void TwoEntityObjectsWithDifferentIdValuesShouldNotBeEqual() {
            var entity1 = new MockEntity(2);
            var entity2 = new MockEntity(3);
            entity1.Should().NotBe(entity2);
        }

        [Fact]
        public void InitializeEntityWithDefaultConstructorShouldSetIdWithDefaultIdValue() {
            var entity = new MockEntity();
            entity.Id.Should().Be(default(long));
        }

        [Theory]
        [ClassData(typeof(HashCodeData))]
        public void CanGenerateCorrectHashCode(MockEntity entity) {
            entity.GetHashCode().Should().Be(entity.Id.GetHashCode());
        }

        private class HashCodeData:TheoryData {
            public HashCodeData() {
                AddRow(new MockEntity(1));
                AddRow(new MockEntity(100));
                AddRow(new MockEntity(10));
                AddRow(new MockEntity(4));
                AddRow(new MockEntity(7));
            }

        }

        public class MockEntity : Entity {
            public MockEntity(long id) : base(id) { }
            public MockEntity() { }
        }
    }

    public abstract class TheoryData : IEnumerable<object[]> {
        readonly List<object[]> data = new List<object[]>();

        protected void AddRow(params object[] values) {
            data.Add(values);
        }

        public IEnumerator<object[]> GetEnumerator() {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
