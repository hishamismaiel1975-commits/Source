using Asp.Versioning;
using Catalog.Application.Products.Responses;
using Catalog.Core.Specifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Platform.Core.Pagination;
using ProductApp = Catalog.Application.Products;


namespace Catalog.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProductById(string id)
        {
            var query = new ProductApp.Queries.GetProductByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductResponse>>> GetProducts([FromQuery] CatalogSpecParams specParams)
        {
            var query = new ProductApp.Queries.GetProductsQuery(specParams);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] ProductApp.Commands.CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, ProductApp.Commands.UpdateProductCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var command = new ProductApp.Commands.DeleteProductCommand(id);
            var result = await _mediator.Send(command);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }

    }
}
