using Catalog.Application.Brands.Responses;
using Catalog.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace Catalog.Application.Brands.Mappers
{
    [Mapper]
    public static partial class BrandMapper
    {
        public static partial BrandResponse ToResponse(ProductBrand brand);
        public static partial IList<BrandResponse> ToResponseList(IEnumerable<ProductBrand> brands);

    }




}
