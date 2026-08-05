using Catalog.Application.Products.Commands;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Products.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            return await _productRepository.DeleteAsync(request.Id);
        }
    }
}
