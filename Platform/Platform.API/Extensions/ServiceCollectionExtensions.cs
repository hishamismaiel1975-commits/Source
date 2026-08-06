using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Platform.API.Exceptions;
using System.Globalization;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlatformServices(
        this IServiceCollection services)
    {
        // Register the global exception handler
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        // Add services to the container.
        services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();

        //Add Swagger services
        services.AddEndpointsApiExplorer();

        // Configure API Versioning
        services
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

        //Add localization services
        services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
        {
                  new CultureInfo("en"),
                  new CultureInfo("ar")
         };

            options.DefaultRequestCulture = new RequestCulture("en");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        return services;
    }
}