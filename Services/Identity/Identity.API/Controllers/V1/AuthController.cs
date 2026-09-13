using Asp.Versioning;
using Identity.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {

        public AuthController()
        {
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            return null;
        }

    }

}
