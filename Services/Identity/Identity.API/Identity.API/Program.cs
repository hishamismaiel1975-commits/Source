using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Platform.Lib.API.Exceptions;
using Platform.Lib.API.Extensions;
using Platform.Lib.Core.Services;
using Platform.Lib.Infrastructure.Services.Localization;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 80 for HTTP requests
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});

//Add Serilog 
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
});

// Register the global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Add Swagger services
builder.Services.AddEndpointsApiExplorer();

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

//Add Swagger services with API versioning support
builder.Services.AddTransient<
 IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions<Program>>();
builder.Services.AddSwaggerGen();

//Add OpenTelemetry services
builder.Services.AddOpenTelemetry()
 .ConfigureResource(resource =>
 {
     resource.AddService(
    typeof(Program).Assembly.GetName().Name!);
 })
.WithTracing(tracing =>
{
    tracing
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation();
    //  .AddOtlpExporter();
})
.WithMetrics(metrics =>
{
    metrics
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation();
    //  .AddOtlpExporter();
})
 .WithLogging(logging =>
 {
     // logging.AddOtlpExporter();
 });

// Add Localization Service
builder.Services.AddSingleton<ILocalizationService, JsonLocalizationService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Enable Swagger
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var apiName = typeof(Program).Assembly.GetName().Name;
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"{apiName} {description.GroupName.ToUpperInvariant()}");
        }
    });
}

app.UseAuthorization();
app.UseExceptionHandler();
app.MapControllers();

app.Run();
