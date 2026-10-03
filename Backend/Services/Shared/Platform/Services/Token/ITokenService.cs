using System.Security.Claims;

namespace Platform.Lib.Services.Token
{
    public interface ITokenService
    {
        string GenerateJwtToken(Guid userId, string tokenType);
        bool ValidateRefreshToken(string refreshToken, out ClaimsPrincipal claimsPrincipal);
    }
}
