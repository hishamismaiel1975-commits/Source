using Catalog.Application.Types.Mappers;
using Catalog.Application.Types.Queries;
using Catalog.Application.Types.Responses;
using Catalog.Core.Persistence.MongoDB.Entities;
using MediatR;
using Platform.Core.Persistence.Repositories;

namespace Catalog.Application.Types.Handlers
{
    public class GetAllTypesHandler : IRequestHandler<GetAllTypesQuery, IList<TypesResponse>>
    {
        private readonly IRepository<ProductType> _typeRepository;

        public GetAllTypesHandler(IRepository<ProductType> repository)
        {
            _typeRepository = repository;
        }
        public async Task<IList<TypesResponse>> Handle(GetAllTypesQuery request, CancellationToken cancellationToken)
        {
            var typesList = await _typeRepository.GetAllAsync();
            return TypeMapper.ToResponseList(typesList);
        }
    }
}
