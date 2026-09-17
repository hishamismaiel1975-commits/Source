using FreeMediator;

namespace Identity.Application.Auth.Commands
{
    public record RegisterCustomerCommand(
        string UserName,
        string FullNameEn,
        string FullNameAr,
        string Password) : IRequest<bool>;
}

