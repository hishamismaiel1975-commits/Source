using Catalog.Application.Brands.Responses;
using Catalog.Core.Persistence.MongoDB.Entities;
using Riok.Mapperly.Abstractions;

namespace Catalog.Application.Brands.Mappers
{
    [Mapper]
    public static partial class BrandMapper
    {
        public static partial BrandResponse ToResponse(Brand brand);
        public static partial IList<BrandResponse> ToResponseList(IEnumerable<Brand> brands);

    }




}
