using Catalog.Application.Products.Commands;
using Catalog.Core.Repositories;
using MediatR;

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
                throw new ApplicationException("FailedToDeleteProduct");
            }
        }
    }
}
