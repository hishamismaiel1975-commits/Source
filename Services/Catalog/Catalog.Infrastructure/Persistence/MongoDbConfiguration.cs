using Catalog.Core.Persistence.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Platform.Core.Persistence.Entities;

namespace Catalog.Infrastructure.Persistence
{
    public static class MongoDbConfiguration
    {
        public static void Configure()
        {
            // Global serializers
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.DateTime));

            // Entity mapping
            BsonClassMap.RegisterClassMap<Entity>(map =>
            {
                map.AutoMap();
                map.MapIdMember(x => x.Id);
            });

            // Product mapping
            BsonClassMap.RegisterClassMap<Product>(map =>
            {
                map.AutoMap();
                map.UnmapMember(x => x.ProductBrand);
                map.UnmapMember(x => x.ProductType);
            });
        }


    }
}

