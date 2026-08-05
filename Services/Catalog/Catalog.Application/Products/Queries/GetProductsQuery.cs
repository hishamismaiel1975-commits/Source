using Catalog.Application.Products.Responses;
using Catalog.Core.Specifications;
using MediatR;
using Platform.Core.Pagination;

namespace Catalog.Application.Products.Queries
{
    public record GetProductsQuery(CatalogSpecParams CatalogSpecParams) : IRequest<Pagination<ProductResponse>>
    {
    }
}
