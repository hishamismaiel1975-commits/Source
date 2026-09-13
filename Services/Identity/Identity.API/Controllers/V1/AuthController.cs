using Asp.Versioning;
using Identity.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Lib.Core.Authorization;

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

        [AllowAnonymous]
        [HttpPost("register-customer")]
        public async Task<IActionResult> RegisterCustomer(RegisterDto registerDto)
        {
            return null;
        }

        [AllowAnonymous]
        [HttpPut("register-customer")]
        public async Task<IActionResult> UpdateCustomer(RegisterDto registerDto)
        {
            //only he can update his own profile
            return null;
        }

        [Authorize(Policy = PermissionConstants.User.Create)]
        [HttpPost("create-employee/{id}")]
        public async Task<IActionResult> CreateEmployee(RegisterDto registerDto)
        {
            return null;
        }

        [Authorize(Policy = PermissionConstants.User.Update)]
        [HttpPut("update-employee/{id}")]
        public async Task<IActionResult> UpdateEmployee(RegisterDto registerDto)
        {
            return null;
        }


        [Authorize(Policy = PermissionConstants.User.Delete)]
        [HttpDelete("delete-employee/{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            return null;
        }

        [Authorize(Policy = PermissionConstants.User.Update)]
        [HttpPut("disactive-employee/{id}")]
        public async Task<IActionResult> DisactiveEmployee(Guid id)
        {
            return null;
        }

        [Authorize(Policy = PermissionConstants.User.Update)]
        [HttpPut("active-employee/{id}")]
        public async Task<IActionResult> ActiveEmployee(Guid id)
        {
            return null;
        }

    }

}
