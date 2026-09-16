using Application.Lib.Core.Services.Discount;
using Application.Lib.Core.Services.Identity.DTOs;
using Application.Lib.Infrastructure.Services.Discount.GrpcClients;
using Identity.GRPC;

namespace Application.Lib.Infrastructure.Services.Identity.GrpcClients
{
    public class IdentityGrpcClient : IIdentityService
    {
        public IdentityService.IdentityServiceClient _IdentityClient { get; set; }
        public IdentityGrpcClient(IdentityService.IdentityServiceClient IdentityClient)
        {
            _IdentityClient = IdentityClient;
        }

        public async Task<UserPermissionsResponse> GetUserPermissionsAsync(Guid productId)
        {
            var response = await _IdentityClient.GetUserPermissionsAsync(new GetUserPermissionsRequest { UserId = productId.ToString() });
            return IdentityGrpcClientMapper.ToDTO(response);
        }
    }
}
