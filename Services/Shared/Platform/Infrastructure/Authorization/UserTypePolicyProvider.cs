using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Platform.Lib.Core.Services.Identity.Enums;

namespace Platform.Lib.Infrastructure.Authorization;

public sealed class UserTypePolicyProvider
    : DefaultAuthorizationPolicyProvider
{
    public UserTypePolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        if (!policyName.StartsWith(
                UserTypeAuthorizeAttribute.PolicyPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            return await base.GetPolicyAsync(policyName);
        }

        var userTypesValue = policyName[
            UserTypeAuthorizeAttribute.PolicyPrefix.Length..];

        if (string.IsNullOrWhiteSpace(userTypesValue))
            return null;

        var userTypes = userTypesValue
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
            {
                return Enum.TryParse<UserTypes>(
                    x,
                    true,
                    out var userType)
                    ? (UserTypes?)userType
                    : null;
            })
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .ToArray();

        if (userTypes.Length == 0)
            return null;

        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(
                new UserTypeRequirement(userTypes))
            .Build();
    }
}