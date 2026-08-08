using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Mappers;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.MongoDB.Repositories;
using MediatR;
using Platform.Core.Exceptions;

namespace Catalog.Application.Products.Handlers
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ITypeRepository _typeRepository;

        public CreateProductHandler(IProductRepository productRepository, IBrandRepository brandRepository, ITypeRepository typeRepository)
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
            var newProduct = await _productRepository.CreateAsync(product);
            if (newProduct == null)
            {
                AppException.Throw("FailedToCreate", $"Failed to create product {request.Name}.");
            }
            return ProductMapper.ToResponse(newProduct);
        }
    }
}
