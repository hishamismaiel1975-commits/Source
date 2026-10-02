using FreeMediator;

namespace Catalog.Application.Products.Commands
{
    public record DeleteProductCommand(
        Guid Id) : IRequest;
}
