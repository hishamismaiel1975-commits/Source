using Asp.Versioning;
using Catalog.Application.Products.Responses;
using Catalog.Core.Specifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Platform.API.Responses;
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
        public async Task<Result<ProductResponse>> GetProductById(string id)
        {
            var query = new ProductApp.Queries.GetProductByIdQuery(id);
            var result = await _mediator.Send(query);
            return Result<ProductResponse>.Success(result);
        }

        [HttpGet]
        public async Task<Result<Pagination<ProductResponse>>> GetProducts([FromQuery] CatalogSpecParams specParams)
        {
            var query = new ProductApp.Queries.GetProductsQuery(specParams);
            var result = await _mediator.Send(query);
            return Result<Pagination<ProductResponse>>.Success(result);
        }

        [HttpPost]
        public async Task<Result<ProductResponse>> CreateProduct([FromBody] ProductApp.Commands.CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return Result<ProductResponse>.Success(result);
        }

        [HttpPut("{id}")]
        public async Task<Result<ProductResponse>> UpdateProduct(string id, ProductApp.Commands.UpdateProductCommand command)
        {
            await _mediator.Send(command);
            return Result<ProductResponse>.Success();
        }

        [HttpDelete()]
        public async Task<Result<ProductResponse>> DeleteProduct(string id)
        {
            var command = new ProductApp.Commands.DeleteProductCommand(id);
            await _mediator.Send(command);
            return Result<ProductResponse>.Success();
        }

    }
}
