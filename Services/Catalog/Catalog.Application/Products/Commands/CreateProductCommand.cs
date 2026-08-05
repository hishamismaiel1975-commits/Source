using Catalog.Application.Products.Responses;
using MediatR;

namespace Catalog.Application.Products.Commands
{
    public record CreateProductCommand(
    string Name,
    string Summary,
    string Description,
    string ImageFile,
    string BrandId,
    string TypeId,
    decimal Price) : IRequest<ProductResponse>;
}
