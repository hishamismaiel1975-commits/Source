using Asp.Versioning;
using Catalog.Application.Types.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Platform.API.Responses;
using TypeApp = Catalog.Application.Types;


namespace Catalog.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TypesController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        public async Task<Result<IList<TypesResponse>>> GetTypes()
        {
            var query = new TypeApp.Queries.GetAllTypesQuery();
            var result = await _mediator.Send(query);
            return Result<IList<TypesResponse>>.Success(result);
        }
    }
}
