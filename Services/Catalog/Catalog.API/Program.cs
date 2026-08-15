using Catalog.Application;
using Catalog.Infrastructure.Data;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Platform.API.Extensions;
using Platform.Core.Persistence.Entities;
using Platform.Core.Persistence.Repositories;
using Platform.Core.Services.Localization;
using Platform.Infrastructure.Persistence.MongoDB.Repositories;
using Platform.Infrastructure.Persistence.MongoDB.Settings;
using Platform.Infrastructure.Services.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add Platform Services
builder.AddPlatform<Program, Application>();

//Register custom Serializers
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

//Register Primary Key for mongo entities
BsonClassMap.RegisterClassMap<Entity>(map =>
{
    map.AutoMap();
    map.MapIdMember(x => x.Id)
       .SetSerializer(new GuidSerializer(BsonType.String));
});

// Bind strongly-typed settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

// Register MongoClient as singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

//Add Custom Services
builder.Services.AddSingleton<ILocalizationService, JsonLocalizationService>();
builder.Services.AddScoped(typeof(ICacheRepository<>), typeof(RedisRepository<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));

//Add Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration["Redis:ConnectionString"];
});

var app = builder.Build();

//Seed Mongo db on startup 
using (var scope = app.Services.CreateScope())
{
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
app.UsePlatform<Program>();

app.Run();

