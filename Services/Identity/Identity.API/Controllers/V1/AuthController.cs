using Asp.Versioning;
using FreeMediator;
using Identity.Application.Auth.Commands;
using Identity.Application.Auth.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Lib.API.Responses;
using Platform.Lib.Core.Services.Security;

namespace Identity.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private IMediator _mediator { get; set; }

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("user/info")]
        public async Task<Result<ICurrentUserService>> UserInfo()
        {
            var result = await _mediator.Send(new UserInfoCommand());
            return Result<ICurrentUserService>.Success(result);
        }

        [AllowAnonymous]
        [HttpPost("token/refresh")]
        public async Task<Result<string>> TokenRefresh([FromBody] string refreshToken)
        {
            var result = await _mediator.Send(new TokenRefreshCommand(refreshToken));
            return Result<string>.Success(result);
        }

        [AllowAnonymous]
        [HttpPost("customer/login")]
        public async Task<Result<LoginResponse>> LoginCustomer(LoginRequest loginRequest)
        {
            var result = await _mediator.Send(new LoginCustomerCommand(loginRequest));
            return Result<LoginResponse>.Success(result);
        }

        [AllowAnonymous]
        [HttpPost("employee/login")]
        public async Task<Result<LoginResponse>> LoginEmployee(LoginRequest loginRequest)
        {
            var result = await _mediator.Send(new LoginEmployeeCommand(loginRequest));
            return Result<LoginResponse>.Success(result);
        }

    }

}
