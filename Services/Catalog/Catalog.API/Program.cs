using Catalog.Application;
using Catalog.Core.Persistence.MongoDB.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Persistence.MongoDB.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Platform.API.Extensions;
using Platform.Core.Services.Localization;
using Platform.Infrastructure.Persistence.Settings;
using Platform.Infrastructure.Services.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add Platform Services
builder.AddPlatform<Program, Application>();

//Register custom Serializers
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

//Add Custom Services
builder.Services.AddSingleton<ILocalizationService, JsonLocalizationService>();

builder.Services.AddScoped(typeof(IRedisRepository<>), typeof(RedisRepository<>));

builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<ITypeRepository, TypeRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();



// Bind strongly-typed settings
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

// Register MongoClient as singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

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
    var options = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseSettings>>();
    await DatabaseSeeder.SeedAsync(options);
}

// Configure the HTTP request pipeline.
app.UsePlatform<Program>();

app.Run();

