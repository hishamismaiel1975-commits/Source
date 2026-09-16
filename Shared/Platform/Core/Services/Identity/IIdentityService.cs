using Platform.Lib.Core.Services.Identity.DTOs;

namespace Platform.Lib.Core.Services.Identity;

public interface IIdentityService
{
    Task<UserPermissionsResponse> GetUserPermissionsAsync(Guid userId);
}
