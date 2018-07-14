using Framework.Services.Contracts.Http;
using SampleApi.V1.Services.Responses.Dto;

namespace SampleApi.V1.Services.Responses
{
    public class PersonResponse:SuccessResponse
    {
        public PersonResponse(PersonDto person)
        {
            Person = person;
        }

        public PersonDto Person { get; }
    }
}