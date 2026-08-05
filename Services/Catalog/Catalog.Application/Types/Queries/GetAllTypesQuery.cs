using Catalog.Application.Types.Responses;
using MediatR;

namespace Catalog.Application.Types.Queries
{
    public record GetAllTypesQuery : IRequest<IList<TypesResponse>>
    {
    }
}
