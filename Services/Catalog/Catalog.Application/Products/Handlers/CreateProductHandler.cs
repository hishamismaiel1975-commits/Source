using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Mappers;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.Entities;
using MediatR;
using Platform.Core.Persistence.Repositories;

namespace Catalog.Application.Products.Handlers
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IRepository<Product> _productRepository;

        public CreateProductHandler(IRepository<Product> productRepository, IRepository<ProductBrand> brandRepository, IRepository<Core.Persistence.Entities.ProductType> typeRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = ProductMapper.ToEntity(request, DateTime.Now);
            await _productRepository.CreateAsync(product);
            return ProductMapper.ToResponse(product);
        }
    }
}
