using Catalog.Application.Products.Commands;
using Catalog.Core.Persistence.Entities;
using MediatR;
using Platform.Core.Persistence.Repositories.MongoDB;

namespace Catalog.Application.Products.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IMongoRepository<Product> _productRepository;

        public DeleteProductHandler(IMongoRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            await _productRepository.DeleteByIdAsync(request.Id);
        }
    }
}
