using Catalog.Application.Brands.Mappers;
using Catalog.Application.Brands.Queries;
using Catalog.Application.Brands.Responses;
using Catalog.Core.Persistence.MongoDB.Entities;
using Catalog.Core.Persistence.MongoDB.Repositories;
using MediatR;

namespace Catalog.Application.Brands.Handlers
{
    public class GetAllBrandsHandler : IRequestHandler<GetAllBrandsQuery, IList<BrandResponse>>
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IRedisRepository<ProductBrand> _redisRepository;

        public GetAllBrandsHandler(IBrandRepository brandRepository, IRedisRepository<ProductBrand> redisRepository)
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
