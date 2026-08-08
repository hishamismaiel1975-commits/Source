using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Platform.API.Exceptions;
using Serilog;
using System.Reflection;

namespace Platform.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplicationBuilder AddPlatform<TProgram, TMediatr>(this WebApplicationBuilder builder)
        {
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

            //Add Serilog 
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File(
                        Path.Combine(AppContext.BaseDirectory, "logs", "app-.log"),
                        rollingInterval: RollingInterval.Day);
            });

            //Add Swagger services with API versioning support
            builder.Services.AddSwaggerGen(options =>
            {
                using var serviceProvider = builder.Services.BuildServiceProvider();

                var provider = serviceProvider
                    .GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(
                        description.GroupName,
                        new OpenApiInfo
                        {
                            Title = typeof(TProgram).Assembly.GetName().Name,
                            Version = description.ApiVersion.ToString()
                        });
                }
            });

            //Add OpenTelemetry services
            builder.Services.AddOpenTelemetry()
             .ConfigureResource(resource =>
             {
                 resource.AddService(
                typeof(TProgram).Assembly.GetName().Name!);
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
            });

            //Register Mediatr
            var assemblies = new Assembly[]
                {
                     Assembly.GetExecutingAssembly(),
                     typeof(TMediatr).Assembly
                };
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

            // Add services to the container.
            builder.Services.AddControllers();

            return builder;
        }

        // Configure the HTTP request pipeline.
        public static WebApplication UsePlatform<TProgram>(this WebApplication app)
        {
            app.UseSerilogRequestLogging();

            // Enable Swagger
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var apiName = typeof(TProgram).Assembly.GetName().Name;
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

            return app;
        }
    }
}
