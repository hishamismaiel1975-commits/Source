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

        public UpdateProductHandler(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existing = await _productRepository.GetByIdAsync(request.Id);
            var updatedProduct = ProductEntityMapper.ToEntity(request);
            updatedProduct.CreatedDate = existing.CreatedDate;
            await _productRepository.UpdateAsync(updatedProduct);
        }
    }

}
