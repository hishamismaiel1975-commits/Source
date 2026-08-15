using Platform.Core.Persistence.Entities;

namespace Catalog.Core.Persistence.MongoDB.Entities
{
    public class ProductBrand : Entity
    {
        public required string Name { get; set; }
    }
}
