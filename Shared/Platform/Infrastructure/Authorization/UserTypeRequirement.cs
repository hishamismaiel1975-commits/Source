using Microsoft.AspNetCore.Authorization;
using Platform.Lib.Core.Services.Identity.Enums;

namespace Platform.Lib.Infrastructure.Authorization;

public sealed class UserTypeRequirement : IAuthorizationRequirement
{
    public IReadOnlyCollection<UserTypes> AllowedUserTypes { get; }

    public UserTypeRequirement(IEnumerable<UserTypes> allowedUserTypes)
    {
        AllowedUserTypes = allowedUserTypes.ToArray();
    }
}