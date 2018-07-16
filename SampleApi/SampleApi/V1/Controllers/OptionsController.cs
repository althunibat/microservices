using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SampleApi.options;

namespace SampleApi.V1.Controllers {
    [ApiVersion("1.0")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class OptionsController : Controller {
        private readonly DataOptions _options;

        public OptionsController(IOptionsSnapshot<DataOptions> options) {
            _options = options.Value;
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult Get() {
            return Ok(_options.Data);
        }
    }
}