using Catalog.Application.Brands.Mappers;
using Catalog.Application.Brands.Queries;
using Catalog.Application.Brands.Responses;
using Catalog.Core.Persistence.MongoDB.Entities;
using MediatR;
using Platform.Core.Persistence.Repositories;

namespace Catalog.Application.Brands.Handlers
{
    public class GetAllBrandsHandler : IRequestHandler<GetAllBrandsQuery, IList<BrandResponse>>
    {
        private readonly IRepository<Brand> _brandRepository;
        private readonly ICacheRepository<Brand> _redisRepository;

        public GetAllBrandsHandler(IRepository<Brand> brandRepository, ICacheRepository<Brand> redisRepository)
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
