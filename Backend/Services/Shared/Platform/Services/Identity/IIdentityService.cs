using Identity.GRPC;

namespace Platform.Lib.Services.Identity;

public interface IIdentityService
{
    Task<GetUserInfoResponse> GetUserInfoAsync(Guid userId);
}
