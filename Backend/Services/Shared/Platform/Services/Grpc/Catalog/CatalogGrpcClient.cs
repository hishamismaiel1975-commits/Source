using Application.Lib.Infrastructure.Services.Discount.GrpcClients;
using Catalog.GRPC;
using Platform.Lib.Services.Grpc.Catalog.DTOs;

namespace Platform.Lib.Services.Grpc.Catalog
{
    public class CatalogGrpcClient : ICatalogService
    {
        public CatalogService.CatalogServiceClient _CatalogClient { get; set; }
        public CatalogGrpcClient(CatalogService.CatalogServiceClient CatalogClient)
        {
            _CatalogClient = CatalogClient;
        }

        public async Task<ProductsDTO[]> GetProductsAsync(Guid? brandId = null, Guid? typeId = null)
        {
            var productsRequest = new GetProductsRequest();
            if (brandId != null) productsRequest.BrandId = brandId.ToString();
            if (typeId != null) productsRequest.TypeId = typeId.ToString();

            var response = await _CatalogClient.GetProductsAsync(productsRequest);
            return CatalogGrpcClientMapper.ToDTO(response.Products);
        }

    }
}
