using FreeMediator;
using Identity.Application.Auth.Commands;
using Platform.Lib.Core.Services.Security;

namespace Identity.Application.Auth.Handlers
{
    public class UserInfoHandler : IRequestHandler<UserInfoCommand, ICurrentUserService>
    {
        private readonly ICurrentUserService _currentUserService;
        public UserInfoHandler(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public async Task<ICurrentUserService> Handle(UserInfoCommand request, CancellationToken cancellationToken)
        {
            return _currentUserService;
        }
    }
}
