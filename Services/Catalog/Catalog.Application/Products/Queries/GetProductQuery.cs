using Catalog.Application.Products.Responses;
using MediatR;

namespace Catalog.Application.Products.Queries
{
    public record GetProductQuery(string Id) : IRequest<ProductResponse>
    {
    }
}
