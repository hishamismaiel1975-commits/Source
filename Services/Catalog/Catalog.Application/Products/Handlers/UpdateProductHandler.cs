using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Mappers;
using Catalog.Core.Persistence.MongoDB.Entities;
using MediatR;
using Platform.Core.Exceptions;
using Platform.Core.Persistence.Repositories;

namespace Catalog.Application.Products.Handlers
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<Core.Persistence.MongoDB.Entities.Type> _typeRepository;

        public UpdateProductHandler(IRepository<Product> productRepository, IRepository<Brand> brandRepository, IRepository<Core.Persistence.MongoDB.Entities.Type> typeRepository)
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
            await _productRepository.UpdateAsync(updatedProduct);
        }
    }

}
