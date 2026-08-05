using MongoDB.Bson.Serialization.Attributes;
using Platform.Core.MongoDB.Entities;

namespace Catalog.Core.Entities
{
    public class ProductType : Entity
    {
        [BsonElement("Name")]
        public required string Name { get; set; }
    }
}
