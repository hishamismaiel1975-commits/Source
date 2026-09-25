using Identity.API.GrpcServices;
using Identity.Application;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Seed;
using Platform.Lib.API.Extensions;
using Platform.Lib.Core.Constants;
using Platform.Lib.Infrastructure.Services.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add Identity gRPC Client to get user permissions from Identity Service
builder.AddIdentityGrpcService();

// Add Platform Services
builder.AddPlatform<Program, App>(PermissionConstants.GetPermissions);

builder.AddGrafanaOTEL("Identity-api");

// Add SQL Server Database Service & SQL Server Repository Services
builder.AddSqlServer<IdentityDbContext>(builder.Configuration["IdentityDb:ConnectionString"]);

// Add Redis Cache Service & Repository Services
builder.AddRedis();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UsePlatform<Program>();

// Add gRPC service to the request pipeline & reflection for postman support
app.MapGrpcService<IdentityGrpcService>();


// Add Built-in Roles, Users, and Update Always New Permissions to the database. 
using (var scope = app.Services.CreateScope())
{
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}


app.Run();
