using Application.Lib.Core.Services.Identity.DTOs;
using Identity.GRPC;
using Riok.Mapperly.Abstractions;

namespace Application.Lib.Infrastructure.Services.Discount.Grpc
{
    [Mapper]
    public static partial class IdentityGrpcClientMapper
    {
        public static partial UserPermissionsResponse ToDTO(GetUserPermissionsResponse getUserPermissionsResponse);

    }
}
