using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Mappers;
using Catalog.Application.Products.Responses;
using Catalog.Application.Resources;
using Catalog.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Catalog.Application.Products.Handlers
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ITypeRepository _typeRepository;
        private readonly IStringLocalizer<Resource> _localizer;

        public CreateProductHandler(IProductRepository productRepository, IBrandRepository brandRepository, ITypeRepository typeRepository, IStringLocalizer<Resource> localizer)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _typeRepository = typeRepository;
            _localizer = localizer;
        }
        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            //Fetch Brand and Type from Repository
            var brand = await _brandRepository.GetByIdAsync(request.BrandId);
            var type = await _typeRepository.GetByIdAsync(request.TypeId);

            if (brand == null || type == null)
            {
                throw new ApplicationException(_localizer["InvalidBrandOrType"]);
            }

            //Match to Entity
            var product = ProductMapper.ToEntity(request, brand, type, DateTimeOffset.UtcNow);
            var newProduct = await _productRepository.CreateAsync(product);
            return ProductMapper.ToResponse(newProduct);
        }
    }
}
