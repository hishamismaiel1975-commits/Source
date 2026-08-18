using Catalog.Application.Products.Mappers;
using Catalog.Application.Products.Queries;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.Entities;
using MediatR;
using Platform.Core.Persistence.Repositories.MongoDB;

namespace Catalog.Application.Products.Handlers
{
    public class GetProductHandler : IRequestHandler<GetProductQuery, ProductResponse>
    {
        private readonly IMongoRepository<Product> _productRepository;

        public GetProductHandler(IMongoRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<ProductResponse> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            return ProductMapper.ToResponse(product);
        }
    }

}
