using Asp.Versioning;
using Identity.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Lib.Core.Authorization;
using Platform.Lib.Core.Services.Security;

namespace Identity.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        public IHashService _hashService { get; set; }
        public IEncryptService _encryptService { get; set; }

        public AuthController(IHashService hashService, IEncryptService encryptService)
        {
            _hashService = hashService;
            _encryptService = encryptService;
        }

        [AllowAnonymous]
        [HttpPost("register-customer")]
        public async Task<IActionResult> RegisterCustomer(RegisterDto registerDto)
        {

            return null;
        }

        [AllowAnonymous]
        [HttpPut("update-customer/{id}")]
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


        [Authorize(Policy = PermissionConstants.User.Update)]
        [HttpPut("active-employee/{id}")]
        public async Task<IActionResult> ActiveEmployee(Guid id)
        {
            return null;
        }

        [Authorize(Policy = PermissionConstants.User.Update)]
        [HttpPut("disactive-employee/{id}")]
        public async Task<IActionResult> DisactiveEmployee(Guid id)
        {
            return null;
        }


    }

}
