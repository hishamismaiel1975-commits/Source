using Core.Services.Discount.DTOs;
using Discount.GRPC;
using Riok.Mapperly.Abstractions;

namespace Infrastructure.Grpc.Discount
{
    [Mapper]
    public static partial class DiscountMapper
    {
        public static partial DiscountDTO ToDTO(GetDiscountResponse discountResponse);

    }
}
