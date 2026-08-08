using Catalog.Core.Persistence.MongoDB.Entities;
using Catalog.Core.Persistence.MongoDB.Repositories;
using Microsoft.Extensions.Options;
using Platform.Infrastructure.Persistence.MongoDB.Repositories;
using Platform.Infrastructure.Persistence.MongoDB.Settings;

namespace Catalog.Infrastructure.Persistence.MongoDB.Repositories
{
    public class TypeRepository : Repository<ProductType>, ITypeRepository
    {
        public TypeRepository(IOptions<DatabaseSettings> options) : base(options)
        {
        }
    }
}
