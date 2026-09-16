using Application.Lib.Core.Services.Discount.DTOs;
using Discount.GRPC;
using Riok.Mapperly.Abstractions;

namespace Application.Lib.Infrastructure.Services.Discount.Grpc
{
    [Mapper]
    public static partial class DiscountGrpcMapper
    {
        public static partial DiscountResponse ToDTO(GetDiscountResponse getDiscountResponse);

    }
}
