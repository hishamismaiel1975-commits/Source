using Discount.GRPC;
using Platform.Lib.Core;
using Riok.Mapperly.Abstractions;

namespace Infrastructure.Grpc.Discount
{
    [Mapper]
    public static partial class DiscountGrpcMapper
    {
        public static partial DiscountResponse ToDTO(GetDiscountResponse discountResponse);

    }
}
