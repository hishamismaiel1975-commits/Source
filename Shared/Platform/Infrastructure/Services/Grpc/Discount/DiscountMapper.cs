using Discount.GRPC;
using Platform.Core.DTOs.Discount;
using Riok.Mapperly.Abstractions;

namespace Infrastructure.Grpc.Discount
{
    [Mapper]
    public static partial class DiscountMapper
    {
        public static partial DiscountResponse ToDTO(GetDiscountResponse discountResponse);

    }
}
