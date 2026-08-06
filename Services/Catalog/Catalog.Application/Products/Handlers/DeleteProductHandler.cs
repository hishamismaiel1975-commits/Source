using Catalog.Application.Products.Commands;
using Catalog.Application.Resources;
using Catalog.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Catalog.Application.Products.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStringLocalizer<Resource> _localizer;

        public DeleteProductHandler(IProductRepository productRepository, IStringLocalizer<Resource> localizer)
        {
            _productRepository = productRepository;
            _localizer = localizer;
        }
        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var status = await _productRepository.DeleteAsync(request.Id);
            if (!status)
            {
                throw new ApplicationException(_localizer["FailedToDeleteProduct", request.Id]);
            }
        }
    }
}
