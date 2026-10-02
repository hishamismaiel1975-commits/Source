using FreeMediator;
using Identity.Application.Auth.Responses;

namespace Identity.Application.Auth.Commands
{
    public record LoginCustomerCommand(
        string UserName,
        string Password) : IRequest<LoginResponse>;
}

