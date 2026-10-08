using Catalog.Infrastructure.Persistence.MongoDB;
using FileStorage.API;
using Platform.Lib.API.Extensions;
using Platform.Lib.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add Platform Services
builder.AddPlatform<Program, App>(PermissionConstants.GetPermissions);

// Add MongoDB Database Service & Configure MongoDB Serializers & MongoDB Repository Services
builder.AddMongoDB(new MongoDbConfiguration());

builder.AddGrafanaOTEL("FileStorage-api");

var app = builder.Build();

app.UsePlatform<Program>();

app.Run();
