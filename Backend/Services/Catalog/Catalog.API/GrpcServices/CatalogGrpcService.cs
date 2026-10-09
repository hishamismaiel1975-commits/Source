using Catalog.Core.Persistence.Entities;
using Catalog.GRPC;
using Grpc.Core;
using Platform.Lib.Persistence.IRepositories;

namespace Catalog.API.GrpcServices;

public class CatalogGrpcService : CatalogService.CatalogServiceBase
{
    public IRepository<Product> _productRepository;
    public ILogger<CatalogGrpcService> _logger { get; set; }

    public CatalogGrpcService(ILogger<CatalogGrpcService> logger, IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public override async Task<GetProductsResult> GetProducts(GetProductsRequest request, ServerCallContext context)
    {
        var products = await _productRepository.GetAllAsync(
            select: x => new GetProductsResponse
            {
                Name = x.Name,
                Summary = x.Summary,
                Description = x.Description,
                BrandName = x.ProductBrand.Name,
                TypeName = x.ProductType.Name,
                Price = x.Price.ToString(),
                CreatedDate = x.CreatedDate.ToString()
            },
            filter: x =>
                 (!request.HasBrandId || x.ProductBrandId.ToString() == request.BrandId)
                 &&
                  (!request.HasTypeId || x.ProductTypeId.ToString() == request.TypeId)
                 );

        return new GetProductsResult()
        {
            Products = { products }
        };
    }


}
