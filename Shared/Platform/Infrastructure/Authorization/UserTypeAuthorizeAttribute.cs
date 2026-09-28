using Microsoft.AspNetCore.Authorization;
using Platform.Lib.Core.Services.Identity.Enums;

namespace Platform.Lib.Infrastructure.Authorization;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = true)]
public sealed class UserTypeAuthorizeAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "UserType:";

    public UserTypeAuthorizeAttribute(params UserTypes[] userTypes)
    {
        if (userTypes == null || userTypes.Length == 0)
            throw new ArgumentException(
                "At least one UserType must be specified.",
                nameof(userTypes));

        Policy = $"{PolicyPrefix}{string.Join(",", userTypes.Select(x => x.ToString()))}";
    }
}