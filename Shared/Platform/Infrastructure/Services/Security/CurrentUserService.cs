using Microsoft.AspNetCore.Http;
using Platform.Lib.Core.Services.Identity.Enums;
using Platform.Lib.Core.Services.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Platform.Lib.Infrastructure.Services.Security;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var userId = User?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(userId, out var id)
                ? id
                : null;
        }
    }
    public string? NameEn =>
        User?.FindFirstValue("name_en");
    public string? NameAr =>
        User?.FindFirstValue("name_ar");
    public UserTypes? UserType
    {
        get
        {
            var value = User?.FindFirstValue("usertype");
            return Enum.TryParse<UserTypes>(value, true, out var userType)
                ? userType
                : null;
        }
    }
    public string? UserTypeName
    {
        get
        {
            var value = User?.FindFirstValue("usertype");
            return Enum.TryParse<UserTypes>(value, true, out var userType)
                ? userType.ToString().ToLower()
                : null;
        }
    }

    public IReadOnlyCollection<string> Permissions =>
        User?
            .FindAll("permission")
            .Select(x => x.Value)
            .Distinct()
            .ToArray()
        ?? Array.Empty<string>();
    public bool HasPermission(string permission) =>
        User?.HasClaim("permission", permission) ?? false;
    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;
}