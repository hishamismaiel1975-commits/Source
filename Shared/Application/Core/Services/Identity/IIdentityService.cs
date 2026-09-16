using Application.Lib.Core.Services.Identity.DTOs;

namespace Application.Lib.Core.Services.Discount;

public interface IIdentityService
{
    Task<UserPermissionsResponse> GetUserPermissionsAsync(Guid userId);
}
