using Catalog.Application.Types.Mappers;
using Catalog.Application.Types.Queries;
using Catalog.Application.Types.Responses;
using Catalog.Core.Constants;
using Catalog.Core.Persistence.Entities;
using FreeMediator;
using Platform.Lib.Core.Persistence.Repositories;

namespace Catalog.Application.Types.Handlers
{
    public class GetAllTypesHandler : IRequestHandler<GetAllTypesQuery, IList<TypesResponse>>
    {
        private readonly IRepository<ProductType> _typeRepository;
        private readonly ICacheRepository<ProductType> _cacheRepository;

        public GetAllTypesHandler(IRepository<ProductType> repository, ICacheRepository<ProductType> cacheRepository)
        {
            _typeRepository = repository;
            _cacheRepository = cacheRepository;
        }
        public async Task<IList<TypesResponse>> Handle(GetAllTypesQuery request, CancellationToken cancellationToken)
        {
            var cachedTypes = await _cacheRepository.GetAllAsync(CacheKeys.AllTypes);

            if (cachedTypes is not null) { return TypeMapper.ToResponseList(cachedTypes); }

            var typesList = await _typeRepository.GetAllAsync();

            // Cache the types list
            await _cacheRepository.SetAllAsync(CacheKeys.AllTypes, typesList, CacheKeys.DefaultExpiration);
            return TypeMapper.ToResponseList(typesList);
        }
    }
}
