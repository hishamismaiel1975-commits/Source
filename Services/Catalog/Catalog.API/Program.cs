using Catalog.Application.Brands.Handlers;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Platform.API.Extensions;
using Platform.Infrastructure.MongoDB.Settings;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add Common Services
builder.Services.AddPlatformServices();

//Register custom Serializers
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

//Add Swagger services
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Catalog API",
        Version = "v1"
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Catalog API",
        Version = "v2"
    });


});

//Register Mediatr
var assemblies = new Assembly[]
    {
        Assembly.GetExecutingAssembly(),
        typeof(GetAllBrandsHandler).Assembly
    };
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

//Add Custom Services
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<ITypeRepository, TypeRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Bind strongly-typed settings
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

// Register MongoClient as singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

var app = builder.Build();

//Seed Mongo db on startup 
using (var scope = app.Services.CreateScope())
{
    var options = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseSettings>>();
    await DatabaseSeeder.SeedAsync(options);
}

// Configure the HTTP request pipeline.
app.UsePlatform();

// Enable Swagger
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog API V1");
        //options.SwaggerEndpoint("/swagger/v2/swagger.json", "Catalog API V2");
    });
}

app.Run();

