using Catalog.Application.Brands.Mappers;
using Catalog.Application.Brands.Queries;
using Catalog.Application.Brands.Responses;
using Catalog.Core.Persistence.MongoDB.Repositories;
using MediatR;

namespace Catalog.Application.Brands.Handlers
{
    public class GetAllBrandsHandler : IRequestHandler<GetAllBrandsQuery, IList<BrandResponse>>
    {
        private readonly IBrandRepository _brandRepository;

        public GetAllBrandsHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }
        public async Task<IList<BrandResponse>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var brandList = await _brandRepository.GetAllAsync();
            return BrandMapper.ToResponseList(brandList);
        }
    }
}
