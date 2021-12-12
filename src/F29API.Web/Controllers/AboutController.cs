using System;

using Microsoft.AspNetCore.Mvc;

using F29API.Services;

namespace F29API.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AboutController : ControllerBase
    {
        private AboutService _aboutService = null;

        public AboutController(AboutService aboutService)
        {
            _aboutService = aboutService;
        }

        [HttpGet]
        public IActionResult Version()
        {
            return Ok(_aboutService.Version);
        }

        [HttpGet]
        public IActionResult InstanceID()
        {
            return Ok($"{_aboutService.InstanceID}");
        }

        [HttpGet]
        public IActionResult Info()
        {
            return Ok(
                new
                {
                    Version = _aboutService.Version,
                    InstanceID = _aboutService.InstanceID,
                    DateTime = DateTime.UtcNow
                }
            );
        }
    }
}
