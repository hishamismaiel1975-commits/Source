using Application.Lib.Core.Constants;
using Identity.Application;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Seed;
using Platform.Lib.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Platform Services
builder.AddPlatform<Program, App>(PermissionConstants.Permissions);

// Add SQL Server Database Service & SQL Server Repository Services
builder.AddSqlServer<IdentityDbContext>(builder.Configuration["IdentityDb:ConnectionString"]);

// Add Redis Cache Service & Repository Services
builder.AddRedis();

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UsePlatform<Program>();

// Add Built-in Roles, Users, and Update Always New Permissions to the database. 
using (var scope = app.Services.CreateScope())
{
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}


app.Run();
