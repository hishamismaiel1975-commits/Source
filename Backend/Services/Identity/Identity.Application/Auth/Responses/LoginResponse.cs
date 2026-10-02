namespace Identity.Application.Auth.Responses
{
    public record LoginResponse
    (
        string AccessToken,
        string RefreshToken
    );
}
