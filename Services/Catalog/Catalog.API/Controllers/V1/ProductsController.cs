using Asp.Versioning;
using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Responses;
using Catalog.Core.Specifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Platform.API.Responses;
using Platform.Core.Models;
using ProductsApp = Catalog.Application.Products;


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
        public async Task<Result<ProductResponse>> GetProductById(string id)
        {
            var query = new ProductsApp.Queries.GetProductQuery(id);
            var result = await _mediator.Send(query);
            return Result<ProductResponse>.Success(result);
        }

        [HttpGet]
        public async Task<Result<Pagination<ProductResponse>>> GetProducts([FromQuery] CatalogSpecParams specParams)
        {
            var query = new ProductsApp.Queries.GetProductsQuery(specParams);
            var result = await _mediator.Send(query);
            return Result<Pagination<ProductResponse>>.Success(result);
        }

        [HttpPost]
        public async Task<Result<ProductResponse>> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return Result<ProductResponse>.Success(result);
        }

        [HttpPut("{id}")]
        public async Task<Result<ProductResponse>> UpdateProduct(string id, UpdateProductCommand command)
        {
            await _mediator.Send(command);
            return Result<ProductResponse>.Success();
        }

        [HttpDelete("{id}")]
        public async Task<Result<ProductResponse>> DeleteProduct(string id)
        {
            var command = new ProductsApp.Commands.DeleteProductCommand(id);
            await _mediator.Send(command);
            return Result<ProductResponse>.Success();
        }

    }
}
