using Catalog.Application;
using Catalog.Infrastructure.Persistence.Seed;
using Catalog.Infrastructure.Persistence.SQLServer;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Platform.Lib.API.Extensions;
using Platform.Lib.Infrastructure.Services.Discount.HttpClients;

var builder = WebApplication.CreateBuilder(args);

// Add Platform Services
builder.AddPlatform<Program, Application>();

// Add MongoDB Database Service & Configure MongoDB Serializers & MongoDB Repository Services
//builder.AddMongoDB(new MongoDbConfiguration());

// Add PostgreSQL Database Service & PostgreSQL Repository Services
//builder.AddPostgreSQL<CatalogPostgresDbContext>();

// Add SQL Server Database Service & SQL Server Repository Services
builder.AddSqlServer<CatalogDbContext>(builder.Configuration["CatalogDb:ConnectionString"]);

// Add Redis Cache Service & Repository Services
builder.AddRedis();


// Add Discount gRPC Service or Add Discount HTTP Client Service
//builder.AddDiscountGrpcService();
builder.AddDiscountHttpClientService();


// Add MassTransit with RabbitMQ and Entity Framework Outbox
builder.Services.AddMassTransit(config =>
{
    config.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
    {
        o.UseBusOutbox();
        o.UseSqlServer();
    });
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

//Seed db on startup 
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

