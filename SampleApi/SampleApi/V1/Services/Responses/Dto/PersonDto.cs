using System.Collections.Generic;
using System.Linq;
using SampleApi.V1.Data.Model;

namespace SampleApi.V1.Services.Responses.Dto
{
    public class PersonDto
    {
        public long Id { get; }
        public string Name { get; }
        public IEnumerable<AddressDto> Address { get; }

        public PersonDto(Person person)
        {
            Id = person.Id;
            Name = person.Name;
            Address = person.Addresses.Select(x => new AddressDto(x));
        }
    }

    public class AddressDto
    {
        public AddressDto(Address address)
        {
            Id = address.Id;
            Country = address.Country;
            City = address.City;
            Street = address.City;
            Building = address.Building;
        }

        public long Id { get; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
    }
}