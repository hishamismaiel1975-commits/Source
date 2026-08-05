using MediatR;

namespace Catalog.Application.Products.Commands
{
    public record DeleteProductCommand(
        string Id) : IRequest<bool>;
}
