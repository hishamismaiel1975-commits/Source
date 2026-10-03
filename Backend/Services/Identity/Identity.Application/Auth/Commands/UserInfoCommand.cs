using FreeMediator;
using Platform.Lib.Services.Security;

namespace Identity.Application.Auth.Commands
{
    public record UserInfoCommand() : IRequest<ICurrentUserService>;
}

