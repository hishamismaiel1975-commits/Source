using Asp.Versioning;
using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Responses;
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
        public async Task<Result<ProductResponse>> GetProductById(Guid id)
        {
            var query = new ProductsApp.Queries.GetProductQuery(id);
            var result = await _mediator.Send(query);
            return Result<ProductResponse>.Success(result);
        }

        [HttpGet]
        public async Task<Result<Pagination<ProductResponse>>> GetProducts(
            string? ProductName,
            string? BrandName,
            string? TypeName,
            Guid? BrandId,
            Guid? TypeId,
            string? SortBy,
            int? PageIndex,
            int? PageSize)
        {
            var query = new ProductsApp.Queries.GetProductsQuery(ProductName, BrandName, TypeName, BrandId, TypeId, SortBy, PageIndex, PageSize);
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
        public async Task<Result<ProductResponse>> UpdateProduct(Guid id, UpdateProductCommand command)
        {
            await _mediator.Send(command);
            return Result<ProductResponse>.Success();
        }

        [HttpDelete("{id}")]
        public async Task<Result<ProductResponse>> DeleteProduct(Guid id)
        {
            var command = new ProductsApp.Commands.DeleteProductCommand(id);
            await _mediator.Send(command);
            return Result<ProductResponse>.Success();
        }

    }
}
