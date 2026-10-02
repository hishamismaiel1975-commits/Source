using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Platform.Lib.Core.Services.Token;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Platform.Lib.Infrastructure.Services.Token
{
    public class TokenService : ITokenService
    {
        public IConfiguration _configuration { get; set; }

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(Guid userId, string tokenType)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim("token_type", tokenType),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Security:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Security:Issuer"],
                audience: _configuration["Security:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>(
                    tokenType == "access_token " ? "Security:AccessTokenLifetimeInMinutes" : "Security:RefreshTokenLifetimeInMinutes")),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public bool ValidateRefreshToken(string refreshToken, out ClaimsPrincipal claimsPrincipal)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Security:SecretKey"])),

                ValidateIssuer = true,
                ValidIssuer = _configuration["Security:Issuer"],

                ValidateAudience = true,
                ValidAudience = _configuration["Security:Audience"],

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(
                    refreshToken,
                    validationParameters,
                    out var validatedToken);

                claimsPrincipal = principal;

                if (validatedToken is not JwtSecurityToken jwtToken)
                    return false;

                // Check token type
                var tokenType = principal.FindFirst("token_type")?.Value;

                if (tokenType != "refresh_token")
                    return false;

                return true;
            }
            catch (SecurityTokenException)
            {
                claimsPrincipal = null;
                return false;
            }
        }
    }
}
