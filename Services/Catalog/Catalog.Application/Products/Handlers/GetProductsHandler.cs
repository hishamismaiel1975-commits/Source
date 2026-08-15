using Catalog.Application.Products.Mappers;
using Catalog.Application.Products.Queries;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.MongoDB.Entities;
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

            if (!string.IsNullOrWhiteSpace(request.ProductName))
                filters.Add(x => x.Name.Contains(request.ProductName));

            if (!string.IsNullOrWhiteSpace(request.BrandName))
                filters.Add(x => x.Brand.Name.Contains(request.BrandName));

            if (!string.IsNullOrWhiteSpace(request.TypeName))
                filters.Add(x => x.Type.Name.Contains(request.TypeName));

            if (request.BrandId != null)
                filters.Add(x => x.Brand.Id == request.BrandId);

            if (request.TypeId != null)
                filters.Add(x => x.Type.Id == request.TypeId);

            var sortMap = new Dictionary<string, Expression<Func<Product, object>>>
            {
                ["name"] = x => x.Name,
                ["price"] = x => x.Price,
                ["brand"] = x => x.Brand.Name,
                ["type"] = x => x.Type.Name
            };

            var products = await _productRepository.GetPagedAsync(filters, request.SortBy, sortMap, request.PageIndex, request.PageSize);
            return ProductMapper.ToResponse(products);
        }
    }
}
