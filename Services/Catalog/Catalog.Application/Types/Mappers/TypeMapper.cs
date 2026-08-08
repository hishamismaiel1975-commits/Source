using Catalog.Application.Types.Responses;
using Catalog.Core.Persistence.MongoDB.Entities;
using Riok.Mapperly.Abstractions;

namespace Catalog.Application.Types.Mappers
{
    [Mapper]
    public static partial class TypeMapper
    {
        public static partial TypesResponse ToResponse(ProductType type);
        public static partial IList<TypesResponse> ToResponseList(IEnumerable<ProductType> types);

    }
}
