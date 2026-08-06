using MediatR;

namespace Catalog.Application.Products.Commands
{
    public record UpdateProductCommand(
    string Id,
    string Name,
    string Summary,
    string Description,
    string ImageFile,
    string BrandId,
    string TypeId,
    decimal Price) : IRequest;

}
