
using Catalog.Core.DTOs;
using Discount.GRPC;
using Riok.Mapperly.Abstractions;

namespace Catalog.Application.Types.Mappers
{
    [Mapper]
    public static partial class DiscountMapper
    {
        public static partial DiscountDTO ToDTO(GetDiscountResponse discountResponse);

    }
}
