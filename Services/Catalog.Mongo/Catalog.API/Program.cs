using Catalog.Application;
using Catalog.Infrastructure.Data;
using Platform.API.Extensions;
using Platform.Core.Persistence.Repositories.Cache;
using Platform.Core.Persistence.Repositories.MongoDB;
using Platform.Core.Services.Localization;
using Platform.Infrastructure.Persistence.MongoDB;
using Platform.Infrastructure.Services.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add Platform Services
builder.AddPlatform<Program, Application>();

// Add MongoDB Services
builder.AddMongoDB();

// Add Redis Cache
builder.AddRedis();

// Add Custom Services
builder.Services.AddSingleton<ILocalizationService, JsonLocalizationService>();
builder.Services.AddScoped(typeof(ICacheRepository<>), typeof(RedisRepository<>));
builder.Services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));


var app = builder.Build();

//Seed Mongo db on startup 
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    using (var scope = app.Services.CreateScope())
    {
        await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
    }
}

// Configure the HTTP request pipeline.
app.UsePlatform<Program>();

app.Run();

