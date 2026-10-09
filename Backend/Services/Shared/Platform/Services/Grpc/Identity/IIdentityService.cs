using Identity.GRPC;

namespace Platform.Lib.Services.Grpc.Identity;

public interface IIdentityService
{
    Task<GetUserInfoResponse> GetUserInfoAsync(Guid userId);
}
