using Asp.Versioning;
using Identity.API.DTOs;
using Identity.Core.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Platform.Lib.API.Responses;
using Platform.Lib.Core.Exceptions;
using Platform.Lib.Core.Persistence.Repositories;
using Platform.Lib.Core.Services.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        public IRepository<User> _userRepository { get; set; }
        public IHashService _hashService { get; set; }
        public IConfiguration _configuration { get; set; }

        public AuthController(IHashService hashService, IRepository<User> userRepository, IConfiguration configuration)
        {
            _hashService = hashService;
            _userRepository = userRepository;
            _configuration = configuration;
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

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("UserType", user.UserType.ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Security:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Security:Issuer"],
                audience: _configuration["Security:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Security:TokenExpirationInMinutes"])),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Result<string>.Success(tokenString);
        }

    }

}
