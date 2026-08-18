using Catalog.Application.Products.Mappers;
using Catalog.Application.Products.Queries;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.Entities;
using MediatR;
using Platform.Core.Models;
using Platform.Core.Persistence.Repositories;
using System.Linq.Expressions;

namespace Catalog.Application.Products.Handlers
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, Pagination<ProductResponse>>
    {
        private readonly IRepository<Product> _productRepository;
        public GetProductsHandler(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<Pagination<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {

            var filters = new List<Expression<Func<Product, bool>>>();

            var includes = new List<Expression<Func<Product, object>>>();

            if (!string.IsNullOrWhiteSpace(request.ProductName))
                filters.Add(x => x.Name.Contains(request.ProductName));

            if (request.BrandId != null)
                filters.Add(x => x.ProductBrandId == request.BrandId);

            if (request.TypeId != null)
                filters.Add(x => x.ProductTypeId == request.TypeId);

            var sortMap = new Dictionary<string, Expression<Func<Product, object>>>
            {
                ["name"] = x => x.Name,
                ["price"] = x => x.Price,
            };


            var products = await _productRepository.GetPagedAsync(filters, includes, request.SortBy, sortMap, request.PageIndex, request.PageSize);
            return ProductMapper.ToResponse(products);
        }
    }
}
