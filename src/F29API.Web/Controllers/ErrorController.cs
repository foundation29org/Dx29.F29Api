using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;

namespace F29API.Web.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult ErrorLocalDevelopment()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return Problem(
                statusCode: 400,
                title: context.Error.Message,
                detail: context.Error.StackTrace
            );
        }
    }
}
