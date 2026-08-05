using Asp.Versioning;
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
using Platform.Infrastructure.MongoDB.Settings;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//Register custom Serializers
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Add Swagger services
builder.Services.AddEndpointsApiExplorer();
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

// Configure API Versioning
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });


var app = builder.Build();

//Seed Mongo db on startup 
using (var scope = app.Services.CreateScope())
{
    var options = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseSettings>>();
    await DatabaseSeeder.SeedAsync(options);
}

// Configure the HTTP request pipeline.

// Enable Swagger
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog API V1");
        //options.SwaggerEndpoint("/swagger/v2/swagger.json", "Catalog API V2");
    });
}


app.UseAuthorization();
app.MapControllers();
app.Run();

