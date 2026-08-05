using Asp.Versioning;
using Catalog.Application.Brands.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BrandApp = Catalog.Application.Brands;


namespace Catalog.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BrandsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet()]
        public async Task<ActionResult<IList<BrandResponse>>> GetBrands()
        {
            var query = new BrandApp.Queries.GetAllBrandsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }


    }
}
