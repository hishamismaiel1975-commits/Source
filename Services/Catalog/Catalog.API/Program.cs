using Application.Lib.Core.Constants;
using Catalog.API.EventBus.Consumer;
using Catalog.Application;
using Catalog.Infrastructure.Persistence.Seed;
using Catalog.Infrastructure.Persistence.SQLServer;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Platform.Lib.API.Extensions;
using Platform.Lib.Infrastructure.Services.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add Identity gRPC Client to get user permissions from Identity Service
builder.AddIdentityGrpcService();

// Add Platform Services
builder.AddPlatform<Program, App>(PermissionConstants.GetPermissions);

// Add MongoDB Database Service & Configure MongoDB Serializers & MongoDB Repository Services
//builder.AddMongoDB(new MongoDbConfiguration());

// Add PostgreSQL Database Service & PostgreSQL Repository Services
//builder.AddPostgreSQL<CatalogPostgresDbContext>();

// Add SQL Server Database Service & SQL Server Repository Services
builder.AddSqlServer<CatalogDbContext>(builder.Configuration["CatalogDb:ConnectionString"]);

// Add Redis Cache Service & Repository Services
builder.AddRedis();


// Add MassTransit with RabbitMQ and Entity Framework Outbox
builder.Services.AddMassTransit(config =>
{
    // PUBLISHER / OUTBOX
    config.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
    {
        o.UseBusOutbox();
        o.UseSqlServer();
    });

    // Consumer
    config.AddConsumer<CreateProductConsumer>();

    // RabbitMQ
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

        // Consumer retry
        cfg.UseMessageRetry(r =>
            r.Interval(3, TimeSpan.FromSeconds(5)));

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

