using Catalog.Application.Products.Mappers;
using Catalog.Application.Products.Queries;
using Catalog.Application.Products.Responses;
using Catalog.Core.Repositories;
using MediatR;
using Platform.Core.Pagination;

namespace Catalog.Application.Products.Handlers
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, Pagination<ProductResponse>>
    {
        private readonly IProductRepository _productRepository;

        public GetProductsHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<Pagination<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {

            var productList = await _productRepository.GetProducts(request.CatalogSpecParams);
            return ProductMapper.ToResponse(productList);
        }
    }
}
