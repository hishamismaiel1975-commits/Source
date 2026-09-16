using Identity.GRPC;
using Platform.Lib.Core.Services.Identity.DTOs;
using Riok.Mapperly.Abstractions;

namespace Application.Lib.Infrastructure.Services.Discount.GrpcClients
{
    [Mapper]
    public static partial class IdentityGrpcClientMapper
    {
        public static partial UserPermissionsResponse ToDTO(GetUserPermissionsResponse getUserPermissionsResponse);

    }
}
