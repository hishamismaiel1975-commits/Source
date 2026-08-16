using Platform.Core.Persistence.Entities;

namespace Catalog.Core.Persistence.MongoDB.Entities
{
    public class Type : Entity
    {
        public required string Name { get; set; }
    }
}
