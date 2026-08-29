using Asp.Versioning;
using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Responses;
using Catalog.Core.DTOs;
using Catalog.Core.Services;
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
        private readonly IDiscountService _discountService;

        public ProductsController(IMediator mediator, IDiscountService discountService)
        {
            _mediator = mediator;
            _discountService = discountService;
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
            Guid? ProductBrandId,
            Guid? ProductTypeId,
            string? SortBy,
            int? PageIndex,
            int? PageSize)
        {
            var query = new ProductsApp.Queries.GetProductsQuery(ProductName, ProductBrandId, ProductTypeId, SortBy, PageIndex, PageSize);
            var result = await _mediator.Send(query);
            return Result<Pagination<ProductResponse>>.Success(result);
        }

        [HttpPost]
        public async Task<Result<ProductResponse>> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return Result<ProductResponse>.Success(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<Result<ProductResponse>> UpdateProduct(Guid id, UpdateProductCommand command)
        {
            await _mediator.Send(command);
            return Result<ProductResponse>.Success();
        }

        [HttpDelete("{id:guid}")]
        public async Task<Result<ProductResponse>> DeleteProduct(Guid id)
        {
            var command = new ProductsApp.Commands.DeleteProductCommand(id);
            await _mediator.Send(command);
            return Result<ProductResponse>.Success();
        }

        [HttpGet("discount/{id:guid}")]
        public async Task<Result<DiscountDTO>> GetProductDiscount(Guid id)
        {
            var response = await _discountService.GetDiscountAsync(id);
            return Result<DiscountDTO>.Success(response);
        }

    }
}
