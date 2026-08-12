using Catalog.Core.Persistence.MongoDB.Entities;
using Catalog.Core.Persistence.MongoDB.Repositories;
using Catalog.Core.Specifications;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Platform.Core.Models;
using Platform.Infrastructure.Persistence.MongoDB.Repositories;
using Platform.Infrastructure.Persistence.Repositories;
using Platform.Infrastructure.Persistence.Settings;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Catalog.Infrastructure.Persistence.MongoDB.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(IOptions<DatabaseSettings> options) : base(options)
        {
        }

        public async Task<Pagination<Product>> GetProducts(CatalogSpecParams catalogSpecParams)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;


            if (!string.IsNullOrWhiteSpace(catalogSpecParams.ProductName))
            {
                filter &= builder.Regex(
                    x => x.Name,
                    new BsonRegularExpression(Regex.Escape(catalogSpecParams.ProductName), "i"));
            }

            if (!string.IsNullOrWhiteSpace(catalogSpecParams.BrandName))
            {
                filter &= builder.Regex(
                    x => x.Brand.Name,
                    new BsonRegularExpression(Regex.Escape(catalogSpecParams.BrandName), "i"));
            }
            if (!string.IsNullOrWhiteSpace(catalogSpecParams.TypeName))
            {
                filter &= builder.Regex(
                    x => x.Type.Name,
                    new BsonRegularExpression(Regex.Escape(catalogSpecParams.TypeName), "i"));
            }



            if (!string.IsNullOrWhiteSpace(catalogSpecParams.BrandId))
            {
                filter &= builder.Eq(x => x.Brand.Id, catalogSpecParams.BrandId);
            }

            if (!string.IsNullOrWhiteSpace(catalogSpecParams.TypeId))
            {
                filter &= builder.Eq(x => x.Type.Id, catalogSpecParams.TypeId);
            }

            var sortMap = new Dictionary<string, Expression<Func<Product, object>>>
            {
                ["name"] = x => x.Name,
                ["price"] = x => x.Price,
                ["brand"] = x => x.Brand.Name,
                ["type"] = x => x.Type.Name
            };

            var sort = catalogSpecParams.Sort ?? "name";
            return await ApplyDataFilters(filter, sortMap, sort, catalogSpecParams.PageIndex, catalogSpecParams.PageSize);

        }


    }
}
