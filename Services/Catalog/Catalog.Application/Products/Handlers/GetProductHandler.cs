using Catalog.Application.Products.Mappers;
using Catalog.Application.Products.Queries;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.MongoDB.Repositories;
using MediatR;
using Platform.Core.Exceptions;

namespace Catalog.Application.Products.Handlers
{
    public class GetProductHandler : IRequestHandler<GetProductQuery, ProductResponse>
    {
        private readonly IProductRepository _productRepository;

        public GetProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<ProductResponse> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                AppException.Throw("ProductNotFound", $"Product with id {request.Id} not found.");
            }

            return ProductMapper.ToResponse(product);

        }
    }
}
