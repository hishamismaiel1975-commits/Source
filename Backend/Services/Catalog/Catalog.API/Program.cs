using Catalog.API.EventBus.Consumer;
using Catalog.Application;
using Catalog.Infrastructure.Persistence.Seed;
using Catalog.Infrastructure.Persistence.SQLServer;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Platform.Lib.API.Extensions;
using Platform.Lib.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add Platform Services
builder.AddPlatform<Program, App>(PermissionConstants.GetPermissions);

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

builder.AddGrafanaOTEL("Catalog-api");

var app = builder.Build();

//Seed db on startup 
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    using (var scope = app.Services.CreateScope())
    {
        await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
    }
}

app.UsePlatform<Program>();

app.Run();


