using Platform.Lib.Persistence.Entities;

namespace Catalog.Core.Persistence.Entities
{
    public class ProductType : Entity
    {
        public required string Name { get; set; }
    }
}
