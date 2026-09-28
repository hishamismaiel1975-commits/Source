using Identity.GRPC;

namespace Platform.Lib.Core.Services.Identity;

public interface IIdentityService
{
    Task<GetUserInfoResponse> GetUserInfoAsync(Guid userId);
}
