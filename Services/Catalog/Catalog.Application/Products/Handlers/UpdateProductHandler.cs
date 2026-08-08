using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Mappers;
using Catalog.Core.Persistence.MongoDB.Repositories;
using MediatR;
using Platform.Core.Exceptions;

namespace Catalog.Application.Products.Handlers
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ITypeRepository _typeRepository;

        public UpdateProductHandler(IProductRepository productRepository, IBrandRepository brandRepository, ITypeRepository typeRepository)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _typeRepository = typeRepository;
        }
        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existing = await _productRepository.GetByIdAsync(request.Id);
            if (existing == null)
            {
                AppException.Throw("ProductNotFound", $"Product with id {request.Id} not found.");
            }
            //Step 1: Fetch Brand and Type
            var brand = await _brandRepository.GetByIdAsync(request.BrandId);
            var type = await _typeRepository.GetByIdAsync(request.TypeId);
            if (brand == null || type == null)
            {
                AppException.Throw("InvalidBrandOrType", $"Invalid Brand Or Type with id {request.BrandId} or {request.TypeId}");
            }

            //Step 2: Mapper Role
            var updatedProduct = ProductMapper.ToEntity(request, brand, type, existing.CreatedDate);

            //Step 3: Save the record
            var status = await _productRepository.UpdateAsync(updatedProduct);
            if (!status)
            {
                AppException.Throw("FailedToUpdate", $"Failed to update product with id {request.Id}");
            }

        }
    }

}
