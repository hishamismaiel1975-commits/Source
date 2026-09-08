using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Mappers;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.Entities;
using FreeMediator;
using MassTransit;
using Microsoft.Extensions.Logging;
using Platform.Core.EventBus.Events;
using Platform.Core.Persistence.Repositories;


namespace Catalog.Application.Products.Handlers
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly ITransactionRepository<Product> _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CreateProductHandler> _logger;

        public CreateProductHandler(ITransactionRepository<Product> productRepository, IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint, ILogger<CreateProductHandler> logger)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = ProductEntityMapper.ToEntity(request);
            _productRepository.Create(product);
            await _publishEndpoint.Publish(new CreateProductEvent
            {
                CorrelationId = Guid.NewGuid(),
                ProductId = product.Id,
                ProjectName = product.Name
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ProductResponseMapper.ToResponse(product);
        }
    }
}
