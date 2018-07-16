using System.Collections.Generic;
using Framework.Model;

namespace SampleApi.V1.Data.Model {
    public class Person : Entity {
        public Person(long id) : base(id) {
            Addresses = new List<Address>();
        }

        protected Person() {
            Addresses = new List<Address>();
        }

        public string Name { get; set; }
        public IList<Address> Addresses { get; set; }
    }
}