using Asp.Versioning;
using Identity.API.DTOs;
using Identity.Core.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Platform.Lib.API.Responses;
using Platform.Lib.Core.Exceptions;
using Platform.Lib.Core.Persistence.Repositories;
using Platform.Lib.Core.Services.Identity.Enums;
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
        private readonly ICurrentUserService _currentUserService;

        public AuthController(IHashService hashService, IRepository<User> userRepository, IConfiguration configuration, ICurrentUserService currentUserService)
        {
            _hashService = hashService;
            _userRepository = userRepository;
            _configuration = configuration;
            _currentUserService = currentUserService;
        }

        [Authorize]
        [HttpGet("user/info")]
        public Result<ICurrentUserService> UserInfo()
        {
            return Result<ICurrentUserService>.Success(_currentUserService);
        }

        [AllowAnonymous]
        [HttpGet("token/refresh/{refreshToken}")]
        public async Task<Result<string>> TokenRefresh(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw AppException.Throw("RefreshTokenIsRequired");

            ClaimsPrincipal principal;
            if (!ValidateRefreshToken(refreshToken, out principal)) throw AppException.Throw("RefreshTokenIsInvalid");

            var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user == null) { AppException.Throw("UserNotFound"); }

            if (!user.IsActive) { AppException.Throw("UserNotFound"); }

            // Generate JWT Access Token
            var accessToken = GenerateJwtToken(user, "access_token");

            return Result<string>.Success(accessToken);
        }


        [AllowAnonymous]
        [HttpPost("customer/login")]
        public async Task<Result<LoginResponse>> LoginCustomer(DTOs.LoginRequest loginRequest)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserName == loginRequest.UserName);

            // Check if the user exists
            if (user == null) { AppException.Throw("InvalidUsernameOrPassword"); }

            // Check if the user is an Customer
            if (user.UserType != UserTypes.Customer) { AppException.Throw("InvalidUsernameOrPassword"); }

            // Verify the password
            if (!_hashService.Verify(loginRequest.Password, user.PasswordHash)) { AppException.Throw("InvalidUsernameOrPassword"); }

            // Generate JWT Access Token
            var accessToken = GenerateJwtToken(user, "accesstoken");
            var refreshToken = GenerateJwtToken(user, "refreshtoken");

            return Result<LoginResponse>.Success(new LoginResponse(accessToken, refreshToken));
        }

        [AllowAnonymous]
        [HttpPost("employee/login")]
        public async Task<Result<LoginResponse>> LoginEmployee(DTOs.LoginRequest loginRequest)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserName == loginRequest.UserName);
            if (user == null) { AppException.Throw("InvalidUsernameOrPassword"); }

            // Check if the user is an employee
            if (user.UserType != UserTypes.Employee) { AppException.Throw("InvalidUsernameOrPassword"); }

            // Verify the password
            if (!_hashService.Verify(loginRequest.Password, user.PasswordHash)) { AppException.Throw("InvalidUsernameOrPassword"); }

            // Generate JWT Access Token
            var accessToken = GenerateJwtToken(user, "access_token");
            var refreshToken = GenerateJwtToken(user, "refresh_token");

            return Result<LoginResponse>.Success(new LoginResponse(accessToken, refreshToken));
        }

        private string GenerateJwtToken(User user, string tokenType)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("token_type", tokenType),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Security:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Security:Issuer"],
                audience: _configuration["Security:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Security:AccessTokenLifetimeInMinutes")),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateRefreshToken(string refreshToken, out ClaimsPrincipal claimsPrincipal)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Security:SecretKey"])),

                ValidateIssuer = true,
                ValidIssuer = _configuration["Security:Issuer"],

                ValidateAudience = true,
                ValidAudience = _configuration["Security:Audience"],

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(
                    refreshToken,
                    validationParameters,
                    out var validatedToken);

                claimsPrincipal = principal;

                if (validatedToken is not JwtSecurityToken jwtToken)
                    return false;

                // Check token type
                var tokenType = principal.FindFirst("token_type")?.Value;

                if (tokenType != "refresh_token")
                    return false;

                return true;
            }
            catch (SecurityTokenException)
            {
                claimsPrincipal = null;
                return false;
            }
        }
    }

}
