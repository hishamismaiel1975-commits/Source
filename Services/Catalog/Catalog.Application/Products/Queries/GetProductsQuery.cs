using Catalog.Application.Products.Responses;
using FreeMediator;
using Platform.Core.Models;

namespace Catalog.Application.Products.Queries
{
    public record GetProductsQuery(
         string? ProductName,
         Guid? BrandId,
         Guid? TypeId,
         string? SortBy,
         int? PageIndex,
         int? PageSize) : IRequest<Pagination<ProductResponse>>;
}
