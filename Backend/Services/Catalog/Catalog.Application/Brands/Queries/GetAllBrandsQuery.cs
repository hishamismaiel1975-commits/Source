using Catalog.Application.Brands.Responses;
using FreeMediator;

namespace Catalog.Application.Brands.Queries
{
    public record GetAllBrandsQuery : IRequest<IList<BrandResponse>>
    {
    }
}
