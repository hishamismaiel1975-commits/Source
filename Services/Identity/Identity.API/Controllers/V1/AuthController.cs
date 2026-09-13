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

        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer(RegisterDto registerDto)
        {
            return null;
        }

        [HttpPost("register/employee")]
        public async Task<IActionResult> RegisterEmployee(RegisterDto registerDto)
        {
            return null;
        }


    }

}
