
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Platform.Core.MongoDB.Entities;

namespace Catalog.Core.Entities
{
    public class Product : Entity
    {
        public required string Name { get; set; }
        public required string Summary { get; set; }
        public required string Description { get; set; }
        public required string ImageFile { get; set; }
        public required ProductBrand Brand { get; set; }
        public required ProductType Type { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public required decimal Price { get; set; }
        public required DateTimeOffset CreatedDate { get; set; }
    }
}
