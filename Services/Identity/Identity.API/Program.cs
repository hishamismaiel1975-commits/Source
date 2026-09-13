using Identity.Application;
using Identity.Infrastructure.Persistence.SQLServer;
using Platform.Lib.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Platform Services
builder.AddPlatform<Program, Application>();

// Add SQL Server Database Service & SQL Server Repository Services
builder.AddSqlServer<IdentityDbContext>();

// Add Redis Cache Service & Repository Services
builder.AddRedis();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UsePlatform<Program>();

app.Run();
