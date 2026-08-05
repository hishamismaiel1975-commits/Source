using MongoDB.Bson.Serialization.Attributes;
using Platform.Core.MongoDB.Entities;

namespace Catalog.Core.Entities
{
    public class ProductBrand : Entity
    {
        [BsonElement("Name")]
        public required string Name { get; set; }




    }
}
