using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Platform.Core.Persistence.Entities;

namespace Catalog.Infrastructure.Persistence.MongoDB;

// Configure MongoDB Serializers and Class Maps
public static class MongoDbConfiguration
{
    public static void Configure()
    {
        RegisterSerializers();
        RegisterClassMaps();
    }

    private static void RegisterSerializers()
    {
        // Global serializers
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
        BsonSerializer.RegisterSerializer(new DateTimeSerializer(BsonType.DateTime));
    }

    private static void RegisterClassMaps()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Entity)))
        {
            BsonClassMap.RegisterClassMap<Entity>(map =>
            {
                map.AutoMap();
                map.MapIdMember(x => x.Id);
            });
        }

    }
}
