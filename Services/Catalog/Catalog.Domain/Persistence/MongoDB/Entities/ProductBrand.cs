using MongoDB.Bson.Serialization.Attributes;
using Platform.Core.Persistence.Entities;

namespace Catalog.Core.Persistence.MongoDB.Entities
{
    public class ProductBrand : MongoEntity
    {
        [BsonElement("Name")]
        public required string Name { get; set; }




    }
}
