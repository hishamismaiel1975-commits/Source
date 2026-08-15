using Catalog.Application.Products.Responses;
using MediatR;
using Platform.Core.Models;

namespace Catalog.Application.Products.Queries
{
    public record GetProductsQuery(
         string? ProductName,
         string? BrandName,
         string? TypeName,
         Guid? BrandId,
         Guid? TypeId,
         string? SortBy,
         int? PageIndex,
         int? PageSize) : IRequest<Pagination<ProductResponse>>;
}
