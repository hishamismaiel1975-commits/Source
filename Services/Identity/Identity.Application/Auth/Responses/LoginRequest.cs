namespace Identity.Application.Auth.Responses
{
    public record LoginRequest
    (
        string UserName,
        string Password
    );
}
