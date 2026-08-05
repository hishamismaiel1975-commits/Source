using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Mappers;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Products.Handlers
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
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
        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existing = await _productRepository.GetByIdAsync(request.Id);
            if (existing == null)
            {
                throw new ApplicationException($"Product with Id {request.Id} not found");
            }
            //Step 1: Fetch Brand and Type
            var brand = await _brandRepository.GetByIdAsync(request.BrandId);
            var type = await _typeRepository.GetByIdAsync(request.TypeId);
            if (brand == null || type == null)
            {
                throw new ApplicationException("Invalid Brand or Type Specified");
            }

            //Step 2: Mapper Role
            var updatedProduct = ProductMapper.ToEntity(request, brand, type, existing.CreatedDate);

            //Step 3: Save the record
            return await _productRepository.UpdateAsync(updatedProduct);
        }
    }

}
