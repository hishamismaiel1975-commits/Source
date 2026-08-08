using MongoDB.Bson.Serialization.Attributes;
using Platform.Core.Persistence.MongoDB.Entities;

namespace Catalog.Core.Persistence.MongoDB.Entities
{
    public class ProductType : Entity
    {
        [BsonElement("Name")]
        public required string Name { get; set; }
    }
}
