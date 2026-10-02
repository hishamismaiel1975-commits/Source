using FreeMediator;

namespace Identity.Application.Auth.Commands
{
    public record TokenRefreshCommand(
        string refreshToken) : IRequest<string>;
}

