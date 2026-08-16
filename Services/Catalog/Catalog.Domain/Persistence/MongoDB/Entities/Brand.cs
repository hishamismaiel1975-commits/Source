using Platform.Core.Persistence.Entities;

namespace Catalog.Core.Persistence.MongoDB.Entities
{
    public class Brand : Entity
    {
        public required string Name { get; set; }
    }
}
