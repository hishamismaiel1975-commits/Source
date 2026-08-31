using Catalog.Application.Types.Responses;
using FreeMediator;

namespace Catalog.Application.Types.Queries
{
    public record GetAllTypesQuery : IRequest<IList<TypesResponse>>
    {
    }
}
