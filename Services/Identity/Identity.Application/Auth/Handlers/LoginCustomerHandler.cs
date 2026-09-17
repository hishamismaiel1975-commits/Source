using FreeMediator;
using Identity.Application.Auth.Commands;
using Identity.Application.Auth.Responses;
using Identity.Core.Persistence.Entities;
using Platform.Lib.Core.Exceptions;
using Platform.Lib.Core.Persistence.Repositories;
using Platform.Lib.Core.Services.Identity.Enums;
using Platform.Lib.Core.Services.Security;
using Platform.Lib.Core.Services.Token;

namespace Identity.Application.Auth.Handlers
{
    public class LoginCustomerHandler : IRequestHandler<LoginCustomerCommand, LoginResponse>
    {
        private ITokenService _tokenService { get; set; }
        public IRepository<User> _userRepository { get; set; }
        public IHashService _hashService { get; set; }

        public LoginCustomerHandler(ITokenService tokenService, IRepository<User> userRepository, IHashService hashService)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _hashService = hashService;
        }

        public async Task<LoginResponse> Handle(LoginCustomerCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserName == request.LoginRequest.UserName);

            // Check if the user exists
            if (user == null) { AppException.Throw("InvalidUsernameOrPassword"); }

            // Check if the user is an Customer
            if (user.UserType != UserTypes.Customer) { AppException.Throw("InvalidUsernameOrPassword"); }

            // Verify the password
            if (!_hashService.Verify(request.LoginRequest.Password, user.PasswordHash)) { AppException.Throw("InvalidUsernameOrPassword"); }


            // Generate JWT Access Token
            var accessToken = _tokenService.GenerateJwtToken(user.Id, "access_token");
            var refreshToken = _tokenService.GenerateJwtToken(user.Id, "refresh_token");

            return new LoginResponse(accessToken, refreshToken);

        }
    }
}
