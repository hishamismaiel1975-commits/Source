using Catalog.Application;
using Catalog.Infrastructure.Grpc;
using Catalog.Infrastructure.Persistence.Seed;
using Catalog.Infrastructure.Persistence.SQLServer;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Platform.API.Extensions;
using Platform.Core.Persistence.Repositories;
using Platform.Core.Services;
using Platform.Infrastructure.Persistence.EFCore.Repositories;
using Platform.Infrastructure.Services.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add Platform Services
builder.AddPlatform<Program, Application>();

// Add MongoDB Database Service & Configure MongoDB Serializers & MongoDB Repository Services
//builder.AddMongoDB();
//MongoDbConfiguration.Configure();
//builder.Services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));

// Add PostgreSQL Database Service & PostgreSQL Repository Services
//builder.AddPostgreSQL<CatalogPostgresDbContext>();
//builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<CatalogPostgresDbContext>());
//builder.Services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));

// Add SQL Server Database Service & SQL Server Repository Services
builder.AddSqlServer<CatalogDbContext>();
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<CatalogDbContext>());
builder.Services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));


// Add Redis Cache Service & Repository Services
builder.AddRedis();
builder.Services.AddScoped(typeof(ICacheRepository<>), typeof(RedisRepository<>));

// Add Other Services
builder.Services.AddSingleton<ILocalizationService, JsonLocalizationService>();

// Add Grpc Client Services
builder.AddGrpcServices();

// Configure MassTransit here
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    });
});

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

