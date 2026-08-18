using Catalog.Application.Brands.Mappers;
using Catalog.Application.Brands.Queries;
using Catalog.Application.Brands.Responses;
using Catalog.Core.Persistence.Entities;
using MediatR;
using Platform.Core.Persistence.Repositories.Cache;
using Platform.Core.Persistence.Repositories.MongoDB;

namespace Catalog.Application.Brands.Handlers
{
    public class GetAllBrandsHandler : IRequestHandler<GetAllBrandsQuery, IList<BrandResponse>>
    {
        private readonly IMongoRepository<ProductBrand> _brandRepository;
        private readonly ICacheRepository<ProductBrand> _redisRepository;

        public GetAllBrandsHandler(IMongoRepository<ProductBrand> brandRepository, ICacheRepository<ProductBrand> redisRepository)
        {
            _brandRepository = brandRepository;
            _redisRepository = redisRepository;
        }
        public async Task<IList<BrandResponse>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            const string cacheKey = "all";

            var cachedBrands = await _redisRepository.GetAllAsync(cacheKey);
            if (cachedBrands is not null)
            {
                return BrandMapper.ToResponseList(cachedBrands);
            }

            var brandList = await _brandRepository.GetAllAsync();

            //Store in Redis
            await _redisRepository.SetAllAsync(cacheKey, brandList, TimeSpan.FromMinutes(5));

            return BrandMapper.ToResponseList(brandList);
        }
    }
}
