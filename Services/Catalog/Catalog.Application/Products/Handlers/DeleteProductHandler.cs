using Catalog.Application.Products.Commands;
using Catalog.Core.Persistence.MongoDB.Repositories;
using MediatR;
using Platform.Core.Exceptions;

namespace Catalog.Application.Products.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var status = await _productRepository.DeleteAsync(request.Id);
            if (!status)
            {
                AppException.Throw("FailedToDeleteProduct", $"Failed to delete product with id {request.Id}.");

            }
        }
    }
}
