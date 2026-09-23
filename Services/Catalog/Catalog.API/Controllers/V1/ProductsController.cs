using Asp.Versioning;
using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Responses;
using FreeMediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Lib.Core.Constants;
using Platform.Lib.Core.DTOs;
using Platform.Lib.Core.Services.Identity.Enums;
using Platform.Lib.Infrastructure.Authorization;
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

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<Result<ProductResponse>> GetProductById(Guid id)
        {
            var query = new ProductsApp.Queries.GetProductQuery(id);
            var result = await _mediator.Send(query);
            return Result<ProductResponse>.Success(result);
        }

        [AllowAnonymous]
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


        [Authorize(Policy = PermissionConstants.Products.Create)]
        //[UserTypeAuthorize(UserTypes.Employee, UserTypes.Customer)]
        [UserTypeAuthorize(UserTypes.Employee)]
        [HttpPost]
        public async Task<Result<ProductResponse>> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return Result<ProductResponse>.Success(result);
        }

        [Authorize(Policy = PermissionConstants.Products.Update)]
        [UserTypeAuthorize(UserTypes.Employee)]
        [HttpPut("{id:guid}")]
        public async Task<Result<ProductResponse>> UpdateProduct(Guid id, UpdateProductCommand command)
        {
            await _mediator.Send(command);
            return Result<ProductResponse>.Success();
        }

        [Authorize(Policy = PermissionConstants.Products.Delete)]
        [UserTypeAuthorize(UserTypes.Employee)]
        [HttpDelete("{id:guid}")]
        public async Task<Result<ProductResponse>> DeleteProduct(Guid id)
        {
            var command = new ProductsApp.Commands.DeleteProductCommand(id);
            await _mediator.Send(command);
            return Result<ProductResponse>.Success();
        }

    }
}
