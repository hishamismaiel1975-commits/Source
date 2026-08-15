using Catalog.Application.Products.Commands;
using Catalog.Core.Persistence.MongoDB.Entities;
using MediatR;
using Platform.Core.Persistence.Repositories;

namespace Catalog.Application.Products.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IRepository<Product> _productRepository;

        public DeleteProductHandler(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            await _productRepository.DeleteByIdAsync(request.Id);
        }
    }
}
