using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace F29API.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public partial class DocumentController : ControllerBase
    {
#if DEBUG
        const string BASE_URL = "http://localhost:9998";
#else
        const string BASE_URL = "http://tika-service";
#endif

        private IHttpClientFactory _clientFactory = null;

        public DocumentController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Version()
        {
            var request = GETRequest("version");

            (var content, var status) = await SendRequest(request);
            if (status == HttpStatusCode.OK)
            {
                return Ok(content);
            }
            return Problem(content, statusCode: (int)status);
        }

        [HttpPut]
        public async Task<IActionResult> Metadata()
        {
            var request = PUTRequest("meta", Request.Body, ("Accept", "application/json"));

            (var content, var status) = await SendRequest(request);
            if (status == HttpStatusCode.OK)
            {
                return Content(content, "application/json", Encoding.UTF8);
            }
            return Problem(content, statusCode: (int)status);
        }
    }
}
