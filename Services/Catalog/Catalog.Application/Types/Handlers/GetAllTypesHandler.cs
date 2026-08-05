using Catalog.Application.Types.Mappers;
using Catalog.Application.Types.Queries;
using Catalog.Application.Types.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Types.Handlers
{
    public class GetAllTypesHandler : IRequestHandler<GetAllTypesQuery, IList<TypesResponse>>
    {
        private readonly ITypeRepository _typeRepository;

        public GetAllTypesHandler(ITypeRepository typeRepository)
        {
            _typeRepository = typeRepository;
        }
        public async Task<IList<TypesResponse>> Handle(GetAllTypesQuery request, CancellationToken cancellationToken)
        {
            var typesList = await _typeRepository.GetAllAsync();
            return TypeMapper.ToResponseList(typesList);
        }
    }
}
