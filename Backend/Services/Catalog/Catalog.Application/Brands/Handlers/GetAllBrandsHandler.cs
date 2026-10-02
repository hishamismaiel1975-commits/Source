using Catalog.Application.Brands.Mappers;
using Catalog.Application.Brands.Queries;
using Catalog.Application.Brands.Responses;
using Catalog.Core.Constants;
using Catalog.Core.Persistence.Entities;
using FreeMediator;
using Platform.Lib.Core.Persistence.Repositories;

namespace Catalog.Application.Brands.Handlers
{
    public class GetAllBrandsHandler : IRequestHandler<GetAllBrandsQuery, IList<BrandResponse>>
    {
        private readonly IRepository<ProductBrand> _brandRepository;
        private readonly ICacheRepository<ProductBrand> _cacheRepository;

        public GetAllBrandsHandler(IRepository<ProductBrand> brandRepository, ICacheRepository<ProductBrand> cacheRepository)
        {
            _brandRepository = brandRepository;
            _cacheRepository = cacheRepository;
        }
        public async Task<IList<BrandResponse>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var cachedBrands = await _cacheRepository.GetAllAsync(CacheKeysConstants.AllBrands);
            if (cachedBrands is not null) { return BrandMapper.ToResponseList(cachedBrands); }

            var brandList = await _brandRepository.GetAllAsync();

            // Cache the brand list
            await _cacheRepository.SetAllAsync(CacheKeysConstants.AllBrands, brandList, CacheKeysConstants.DefaultExpiration);
            return BrandMapper.ToResponseList(brandList);
        }
    }
}
