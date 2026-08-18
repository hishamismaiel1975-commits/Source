using Catalog.Application.Types.Responses;
using Riok.Mapperly.Abstractions;

namespace Catalog.Application.Types.Mappers
{
    [Mapper]
    public static partial class TypeMapper
    {
        public static partial TypesResponse ToResponse(Core.Persistence.Entities.ProductType type);
        public static partial IList<TypesResponse> ToResponseList(IEnumerable<Core.Persistence.Entities.ProductType> types);

    }
}
