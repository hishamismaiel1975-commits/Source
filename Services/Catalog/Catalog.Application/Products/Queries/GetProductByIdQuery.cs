using Catalog.Application.Products.Responses;
using MediatR;

namespace Catalog.Application.Products.Queries
{
    public record GetProductByIdQuery(string Id) : IRequest<ProductResponse>
    {
    }
}
