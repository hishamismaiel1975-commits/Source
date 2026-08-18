using MongoDB.Bson.Serialization.Attributes;
using Platform.Core.Persistence.Entities;

namespace Catalog.Core.Persistence.Entities
{
    public class Product : Entity
    {
        public required string Name { get; set; }
        public required string Summary { get; set; }
        public required string Description { get; set; }
        public required string ImageFile { get; set; }
        public required Guid ProductBrandId { get; set; }
        public required Guid ProductTypeId { get; set; }
        public required decimal Price { get; set; }
        public required DateTime CreatedDate { get; set; }

        [BsonIgnore]
        public ProductBrand? ProductBrand { get; set; }

        [BsonIgnore]
        public ProductType? ProductType { get; set; }

    }
}
