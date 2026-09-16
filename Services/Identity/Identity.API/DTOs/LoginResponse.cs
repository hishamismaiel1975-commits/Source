namespace Identity.API.DTOs
{
    public record LoginResponse
    (
        string AccessToken,
        string RefreshToken
    );
}
