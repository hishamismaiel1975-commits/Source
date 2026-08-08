using Catalog.Core.Persistence.MongoDB.Entities;
using Catalog.Core.Specifications;
using Platform.Core.Models;
using Platform.Core.Persistence.MongoDB.Repositories;

namespace Catalog.Core.Persistence.MongoDB.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Pagination<Product>> GetProducts(CatalogSpecParams specParams);

    }
}
