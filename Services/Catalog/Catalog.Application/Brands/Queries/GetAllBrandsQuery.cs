using Catalog.Application.Brands.Responses;
using MediatR;

namespace Catalog.Application.Brands.Queries
{
    public record GetAllBrandsQuery : IRequest<IList<BrandResponse>>
    {
    }
}
