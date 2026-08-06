using Asp.Versioning;
using Catalog.Application.Brands.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Platform.API.Responses;
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
        public async Task<Result<IList<BrandResponse>>> GetBrands()
        {
            var query = new BrandApp.Queries.GetAllBrandsQuery();
            var result = await _mediator.Send(query);
            return Result<IList<BrandResponse>>.Success(result);
        }

        [HttpGet("GetBrands2")]
        public async Task<Result<IList<BrandResponse>>> GetBrands2()
        {
            throw new ApplicationException("This is a test ApplicationException");

        }

        [HttpGet("GetBrands3")]
        public async Task<Result<IList<BrandResponse>>> GetBrands3()
        {
            throw new Exception("This is a test Exception");

        }



    }
}
