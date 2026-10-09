using Catalog.GRPC;
using Google.Protobuf.Collections;
using Platform.Lib.Services.Grpc.Catalog.DTOs;
using Riok.Mapperly.Abstractions;

namespace Application.Lib.Infrastructure.Services.Discount.GrpcClients
{
    [Mapper]
    public static partial class CatalogGrpcClientMapper
    {
        public static partial ProductsDTO ToDTO(GetProductsResponse source);
        public static ProductsDTO[] ToDTO(RepeatedField<GetProductsResponse> source) => source.Select(ToDTO).ToArray();

    }
}
