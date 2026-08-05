using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.Extensions.Options;
using Platform.Infrastructure.MongoDB.Repositories;
using Platform.Infrastructure.MongoDB.Settings;

namespace Catalog.Infrastructure.Repositories
{
    public class TypeRepository : Repository<ProductType>, ITypeRepository
    {
        public TypeRepository(IOptions<DatabaseSettings> options) : base(options)
        {
        }
    }
}
