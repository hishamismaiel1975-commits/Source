using Catalog.Core.Persistence.MongoDB.Entities;
using Platform.Core.Persistence.Repositories;

namespace Catalog.Core.Persistence.MongoDB.Repositories
{
    public interface IBrandRepository : IMongoRepository<ProductBrand>
    {

    }
}
