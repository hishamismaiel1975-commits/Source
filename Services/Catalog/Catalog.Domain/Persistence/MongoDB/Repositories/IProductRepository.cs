using Catalog.Core.Persistence.MongoDB.Entities;
using Catalog.Core.Specifications;
using Platform.Core.Models;
using Platform.Core.Persistence.Repositories;

namespace Catalog.Core.Persistence.MongoDB.Repositories
{
    public interface IProductRepository : IMongoRepository<Product>
    {
        Task<Pagination<Product>> GetProducts(CatalogSpecParams specParams);

    }
}
