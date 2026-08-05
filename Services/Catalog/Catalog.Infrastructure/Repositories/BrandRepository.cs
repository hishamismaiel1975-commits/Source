using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.Extensions.Options;
using Platform.Infrastructure.MongoDB.Repositories;
using Platform.Infrastructure.MongoDB.Settings;

namespace Catalog.Infrastructure.Repositories
{
    public class BrandRepository : Repository<ProductBrand>, IBrandRepository
    {
        public BrandRepository(IOptions<DatabaseSettings> options) : base(options)
        {
        }
    }
}
