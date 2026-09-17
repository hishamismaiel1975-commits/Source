using Identity.GRPC;
using Platform.Lib.Core.Services.Identity;

namespace Platform.Lib.Infrastructure.Services.Identity
{
    public class IdentityGrpcClient : IIdentityService
    {
        public IdentityService.IdentityServiceClient _IdentityClient { get; set; }
        public IdentityGrpcClient(IdentityService.IdentityServiceClient IdentityClient)
        {
            _IdentityClient = IdentityClient;
        }

        public async Task<GetUserInfoResponse> GetUserInfoAsync(Guid userId)
        {
            var response = await _IdentityClient.GetUserInfoAsync(new GetUserInfoRequest { UserId = userId.ToString() });
            return response;
        }

    }
}
