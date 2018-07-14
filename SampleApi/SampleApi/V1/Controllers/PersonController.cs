using System.Linq;
using System.Threading.Tasks;
using Framework.Services.Contracts.Http;
using Microsoft.AspNetCore.Mvc;
using SampleApi.V1.Services;
using SampleApi.V1.Services.Responses;

namespace SampleApi.V1.Controllers
{
    [ApiVersion("1.0"), Route("api/v{api-version:apiVersion}/[controller]")]
    public class PersonController : Controller
    {
        private readonly IPersonService _service;

        public PersonController(IPersonService service)
        {
            _service = service;
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(PersonResponse), 200)]
        [ProducesResponseType(typeof(FailureResponse), 400)]
        public async Task<IActionResult> Get(long id)
        {
            var requesterId = User.Identity.IsAuthenticated
                ? User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                : "anonymous";
            var request = new ByIdRequest<long>(id);
            request.SetRequesterId(requesterId);
            var result = await _service.GetById(request).ConfigureAwait(false);
            return result.Success ? Ok(result) : (IActionResult)BadRequest(result);
        }
    }
}