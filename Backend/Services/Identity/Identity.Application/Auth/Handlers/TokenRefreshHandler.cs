using FreeMediator;
using Identity.Application.Auth.Commands;
using Identity.Core.Persistence.Entities;
using Platform.Lib.Exceptions;
using Platform.Lib.Persistence.IRepositories;
using Platform.Lib.Services.Security;
using Platform.Lib.Services.Token;
using System.Security.Claims;

namespace Identity.Application.Auth.Handlers
{
    public class TokenRefreshHandler : IRequestHandler<TokenRefreshCommand, string>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITokenService _TokenService;
        public IRepository<User> _userRepository { get; set; }

        public TokenRefreshHandler(ICurrentUserService currentUserService, ITokenService tokenService, IRepository<User> userRepository)
        {
            _currentUserService = currentUserService;
            _TokenService = tokenService;
            _userRepository = userRepository;
        }

        public async Task<string> Handle(TokenRefreshCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.refreshToken))
                throw AppException.Throw("RefreshTokenIsRequired");

            ClaimsPrincipal principal;
            if (!_TokenService.ValidateRefreshToken(request.refreshToken, out principal)) throw AppException.Throw("RefreshTokenIsInvalid");

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user == null) { AppException.Throw("UserNotFound"); }

            if (!user.IsActive) { AppException.Throw("UserNotFound"); }

            // Generate JWT Access Token
            var accessToken = _TokenService.GenerateJwtToken(user.Id, "access_token");

            return accessToken;
        }
    }
}
