using Catalog.GRPC;

namespace Platform.Lib.Services.Grpc.Identity
{
    public class CatalogGrpcClient : ICatalogService
    {
        public CatalogService.CatalogServiceClient _CatalogClient { get; set; }
        public CatalogGrpcClient(CatalogService.CatalogServiceClient CatalogClient)
        {
            _CatalogClient = CatalogClient;
        }

        public async Task<GetProductsResult> GetProductsAsync(Guid? brandId, Guid? typeId)
        {
            var response = await _CatalogClient.GetProductsAsync(new GetProductsRequest { BrandId = brandId?.ToString(), TypeId = typeId?.ToString() });
            return response;
        }

    }
}
