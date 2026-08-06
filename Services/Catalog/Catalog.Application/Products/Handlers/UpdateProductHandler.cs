using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Mappers;
using Catalog.Application.Resources;
using Catalog.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Catalog.Application.Products.Handlers
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ITypeRepository _typeRepository;

        private readonly IStringLocalizer<Resource> _localizer;

        public UpdateProductHandler(IProductRepository productRepository, IBrandRepository brandRepository, ITypeRepository typeRepository, IStringLocalizer<Resource> localizer)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _typeRepository = typeRepository;
            _localizer = localizer;
        }
        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existing = await _productRepository.GetByIdAsync(request.Id);
            if (existing == null)
            {
                throw new ApplicationException(_localizer["ProductNotFound", request.Id]);
            }
            //Step 1: Fetch Brand and Type
            var brand = await _brandRepository.GetByIdAsync(request.BrandId);
            var type = await _typeRepository.GetByIdAsync(request.TypeId);
            if (brand == null || type == null)
            {
                throw new ApplicationException(_localizer["InvalidBrandOrType"]);
            }

            //Step 2: Mapper Role
            var updatedProduct = ProductMapper.ToEntity(request, brand, type, existing.CreatedDate);

            //Step 3: Save the record
            var status = await _productRepository.UpdateAsync(updatedProduct);
            if (!status)
            {
                throw new ApplicationException(_localizer["FailedToUpdateProduct", request.Id]);
            }

        }
    }

}
