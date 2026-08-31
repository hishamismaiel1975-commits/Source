using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Mappers;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.Entities;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Platform.Core.Persistence.Repositories;


namespace Catalog.Application.Products.Handlers
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CreateProductHandler> _logger;

        public CreateProductHandler(IRepository<Product> productRepository, IPublishEndpoint publishEndpoint, ILogger<CreateProductHandler> logger)
        {
            _productRepository = productRepository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = ProductEntityMapper.ToEntity(request);
            await _productRepository.CreateAsync(product);
            await _publishEndpoint.Publish(new CreateProductEvent
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                ProjectName = product.Name
            }, cancellationToken);

            _logger.LogInformation("Product created and event published: {ProductId}", product.Id);

            return ProductResponseMapper.ToResponse(product);
        }
    }
}
