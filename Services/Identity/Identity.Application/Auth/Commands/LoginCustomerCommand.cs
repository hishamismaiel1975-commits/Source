using FreeMediator;
using Identity.Application.Auth.Responses;

namespace Identity.Application.Auth.Commands
{
    public record LoginCustomerCommand(
        LoginRequest LoginRequest) : IRequest<LoginResponse>;
}

