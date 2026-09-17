using FreeMediator;
using Identity.Application.Auth.Responses;

namespace Identity.Application.Auth.Commands
{
    public record LoginEmployeeCommand(
        LoginRequest LoginRequest) : IRequest<LoginResponse>;
}

