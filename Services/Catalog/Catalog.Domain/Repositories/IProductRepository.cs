using Catalog.Core.Entities;
using Catalog.Core.Specifications;
using Platform.Core.MongoDB.Repositories;
using Platform.Core.Pagination;

namespace Catalog.Core.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Pagination<Product>> GetProducts(CatalogSpecParams specParams);

    }
}
