using MediatR;

namespace Catalog.Application.Products.Commands
{
    public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Summary,
    string Description,
    string ImageFile,
    Guid ProductBrandId,
    Guid ProductTypeId,
    decimal Price) : IRequest;

}
