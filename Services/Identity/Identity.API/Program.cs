using Identity.Application;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Seed;
using Platform.Lib.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Platform Services
builder.AddPlatform<Program, Application>();

// Add SQL Server Database Service & SQL Server Repository Services
builder.AddSqlServer<IdentityDbContext>();

// Add Redis Cache Service & Repository Services
builder.AddRedis();



var app = builder.Build();

// Add Built-in Roles, Users, and Update Always New Permissions to the database. 
using (var scope = app.Services.CreateScope())
{
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
app.UsePlatform<Program>();

app.Run();
