using Microsoft.AspNetCore.Authorization;
using Platform.Lib.Core.Services.Identity.Enums;

namespace Platform.Lib.Infrastructure.Authorization;

public sealed class UserTypeAuthorizationHandler
    : AuthorizationHandler<UserTypeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserTypeRequirement requirement)
    {
        var userTypeClaim = context.User.FindFirst("usertype");

        if (userTypeClaim == null)
            return Task.CompletedTask;

        if (!Enum.TryParse<UserTypes>(
                userTypeClaim.Value,
                ignoreCase: true,
                out var userType))
        {
            return Task.CompletedTask;
        }

        if (requirement.AllowedUserTypes.Contains(userType))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}