using Catalog.Application.Products.Mappers;
using Catalog.Application.Products.Queries;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.Entities;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using Platform.Core.Models;
using Platform.Core.Persistence.Repositories.MongoDB;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Catalog.Application.Products.Handlers
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, Pagination<ProductResponse>>
    {
        private readonly IMongoRepository<Product> _productRepository;
        public GetProductsHandler(IMongoRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<Pagination<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {

            var filters = new List<FilterDefinition<Product>>();
            var includes = new List<Expression<Func<Product, object>>>();

            var builder = Builders<Product>.Filter;

            if (!string.IsNullOrWhiteSpace(request.ProductName))
            {
                filters.Add(builder.Regex(x => x.Name, new BsonRegularExpression(Regex.Escape(request.ProductName), "i")));

            }

            if (request.BrandId != null)
            {
                filters.Add(builder.Eq(x => x.ProductBrandId, request.BrandId));
            }

            if (request.TypeId != null)
            {
                filters.Add(builder.Eq(x => x.ProductTypeId, request.TypeId));
            }

            var sortMap = new Dictionary<string, Expression<Func<Product, object>>>
            {
                ["name"] = x => x.Name,
                ["price"] = x => x.Price
            };

            var products = await _productRepository.GetPagedAsync(filters, includes, request.SortBy, sortMap, request.PageIndex, request.PageSize);
            return ProductMapper.ToResponse(products);
        }
    }
}



