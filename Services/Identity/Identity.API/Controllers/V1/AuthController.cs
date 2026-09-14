using Asp.Versioning;
using Identity.API.DTOs;
using Identity.Core.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Lib.API.Responses;
using Platform.Lib.Core.Exceptions;
using Platform.Lib.Core.Persistence.Repositories;
using Platform.Lib.Core.Services.Security;

namespace Identity.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        public IRepository<User> _userRepository { get; set; }
        public IHashService _hashService { get; set; }

        public AuthController(IHashService hashService, IRepository<User> userRepository)
        {
            _hashService = hashService;
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost("customer/register")]
        public async Task<IActionResult> RegisterCustomer(RegisterDto registerDto)
        {

            return null;
        }

        [Authorize]
        [HttpPut("customer/update/{id}")]
        public async Task<IActionResult> UpdateCustomer(RegisterDto registerDto)
        {
            //only he can update his own profile
            return null;
        }


        [AllowAnonymous]
        [HttpPost("customer/login")]
        public async Task<IActionResult> LoginCustomer(LoginDto loginDto)
        {
            //only he can update his own profile
            return null;
        }

        [AllowAnonymous]
        [HttpPost("employee/login")]
        public async Task<Result<string>> LoginEmployee(LoginDto loginDto)
        {

            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName);
            if (user == null) { AppException.Throw("InvalidUsernameOrPassword"); }
            if (!_hashService.Verify(loginDto.Password, user.PasswordHash)) { AppException.Throw("InvalidUsernameOrPassword"); }


            return Result<string>.Success("Login successful");
        }

    }

}
