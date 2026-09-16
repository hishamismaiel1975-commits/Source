namespace Identity.API.DTOs
{
    public record LoginRequest
    (
        string UserName,
        string Password
    );
}
