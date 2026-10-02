using Catalog.Application.Products.Responses;
using FreeMediator;

namespace Catalog.Application.Products.Queries
{
    public record GetProductQuery(Guid Id) : IRequest<ProductResponse>
    {
    }
}
