using Platform.Lib.API.Extensions;
using Platform.Lib.Constants;
using Platform.Lib.Services.Grpc.Catalog;
using QuestPDF.Infrastructure;
using Reporting.API;
using Reporting.Infrastructure.Persistence.MongoDB;
using Reporting.Infrastructure.Persistence.Seed;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add Grpc Client Service 
builder.AddCatalogGrpcClientService();


// Add Platform Services
builder.AddPlatform<Program, App>(PermissionConstants.GetPermissions);


// Add MongoDB Database Service & Configure MongoDB Serializers & MongoDB Repository Services
builder.AddMongoDB(new MongoDbConfiguration());


// Add Grafana OTEL
builder.AddGrafanaOTEL("Reporting.API");

var app = builder.Build();

//Seed db on startup 
using (var scope = app.Services.CreateScope())
{
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}

app.UsePlatform<Program>();

app.Run();
