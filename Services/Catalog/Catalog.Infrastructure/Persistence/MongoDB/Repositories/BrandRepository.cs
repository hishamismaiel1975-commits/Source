using Catalog.Core.Persistence.MongoDB.Entities;
using Catalog.Core.Persistence.MongoDB.Repositories;
using Microsoft.Extensions.Options;
using Platform.Infrastructure.Persistence.MongoDB.Repositories;
using Platform.Infrastructure.Persistence.Repositories;
using Platform.Infrastructure.Persistence.Settings;

namespace Catalog.Infrastructure.Persistence.MongoDB.Repositories
{
    public class BrandRepository : Repository<ProductBrand>, IBrandRepository
    {
        public BrandRepository(IOptions<DatabaseSettings> options) : base(options)
        {
        }
    }
}
