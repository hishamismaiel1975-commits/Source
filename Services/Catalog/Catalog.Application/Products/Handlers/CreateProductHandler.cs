using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Mappers;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.MongoDB.Entities;
using MediatR;
using Platform.Core.Exceptions;
using Platform.Core.Persistence.Repositories;

namespace Catalog.Application.Products.Handlers
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<Core.Persistence.MongoDB.Entities.Type> _typeRepository;

        public CreateProductHandler(IRepository<Product> productRepository, IRepository<Brand> brandRepository, IRepository<Core.Persistence.MongoDB.Entities.Type> typeRepository)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _typeRepository = typeRepository;
        }
        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            //Fetch Brand and Type from Repository
            var brand = await _brandRepository.GetByIdAsync(request.BrandId);
            var type = await _typeRepository.GetByIdAsync(request.TypeId);

            if (brand == null || type == null)
            {
                AppException.Throw("InvalidBrandOrType", $"Invalid Brand Or Type with id {request.BrandId} or {request.TypeId}");
            }

            //Match to Entity
            var product = ProductMapper.ToEntity(request, brand, type, DateTimeOffset.UtcNow);
            await _productRepository.CreateAsync(product);
            return ProductMapper.ToResponse(product);
        }
    }
}
