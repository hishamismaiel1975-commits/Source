using FreeMediator;
using Platform.Lib.Core.Services.Security;

namespace Identity.Application.Auth.Commands
{
    public record UserInfoCommand() : IRequest<ICurrentUserService>;
}

