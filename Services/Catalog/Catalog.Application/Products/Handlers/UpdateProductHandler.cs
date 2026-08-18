using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Mappers;
using Catalog.Core.Persistence.Entities;
using MediatR;
using Platform.Core.Persistence.Repositories;

namespace Catalog.Application.Products.Handlers
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductBrand> _brandRepository;
        private readonly IRepository<Core.Persistence.Entities.ProductType> _typeRepository;

        public UpdateProductHandler(IRepository<Product> productRepository, IRepository<ProductBrand> brandRepository, IRepository<Core.Persistence.Entities.ProductType> typeRepository)
        {
            _productRepository = productRepository;
        }
        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existing = await _productRepository.GetByIdAsync(request.Id);
            var updatedProduct = ProductMapper.ToEntity(request, existing.CreatedDate);
            await _productRepository.UpdateAsync(updatedProduct);
        }
    }

}
