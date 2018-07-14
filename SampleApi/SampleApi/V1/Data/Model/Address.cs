using Framework.Model;

namespace SampleApi.V1.Data.Model
{
    public class Address:Entity
    {
        public Address(long id) : base(id)
        {
        }

        protected Address()
        {
        }

        public AddressType Type { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public long PersonId { get; set; }
        public Person Person { get; set; }
    }
}