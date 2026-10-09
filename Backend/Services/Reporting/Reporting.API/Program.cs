using Platform.Lib.API.Extensions;
using Platform.Lib.Constants;
using Platform.Lib.Services.Grpc.Catalog;
using Reporting.API;

var builder = WebApplication.CreateBuilder(args);

// Add Grpc Client Service 
builder.AddCatalogGrpcClientService();


// Add Platform Services
builder.AddPlatform<Program, App>(PermissionConstants.GetPermissions);

// Add Grafana OTEL
builder.AddGrafanaOTEL("Reporting.API");

var app = builder.Build();

//Seed db on startup 
using (var scope = app.Services.CreateScope())
{
    //await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}

app.UsePlatform<Program>();

app.Run();
