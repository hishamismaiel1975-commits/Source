using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using FreeMediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Driver;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Platform.Lib.API.Exceptions;
using Platform.Lib.API.Swagger;
using Platform.Lib.Application.Behaviors;
using Platform.Lib.Core.Persistence.MongoDB;
using Platform.Lib.Core.Persistence.Repositories;
using Platform.Lib.Core.Services.Localization;
using Platform.Lib.Core.Services.Security;
using Platform.Lib.Infrastructure.Persistence.EFCore.Interceptors;
using Platform.Lib.Infrastructure.Persistence.EFCore.Repositories;
using Platform.Lib.Infrastructure.Persistence.MongoDB.Repositories;
using Platform.Lib.Infrastructure.Services.Localization;
using Platform.Lib.Infrastructure.Services.Security;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;


namespace Platform.Lib.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplicationBuilder AddPlatform<TProgram, TMediatr>(this WebApplicationBuilder builder, IEnumerable<string?> permissions)
        {
            // Configure Kestrel to listen on different ports for HTTP/1.1 and HTTP/2
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(80, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });

                options.ListenAnyIP(81, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });

            builder.Services.AddGrpc();
            builder.Services.AddGrpcReflection();

            // Register Auditing & CurrentUser services
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<AuditingSaveChangesInterceptor>();

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

            //Stop auto model validation to enable mediator validation
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

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
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions<TProgram>>();
            builder.Services.AddSwaggerGen(options =>
            {
                options.OperationFilter<AcceptLanguageHeaderOperationFilter>();

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token. Example: Bearer {token}"
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
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
            })
             .WithLogging(logging =>
             {
                 // logging.AddOtlpExporter();
             });

            //Register FreeMediator 
            builder.Services.AddMediator(config =>
            {
                var assemblies = new Assembly[] { Assembly.GetExecutingAssembly(), typeof(TMediatr).Assembly };
                config.RegisterServicesFromAssemblies(assemblies);
            });
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.AddValidatorsFromAssemblyContaining<TMediatr>();

            // Add JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(options =>
              {
                  options.RequireHttpsMetadata = false;
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,

                      ValidIssuer = builder.Configuration["Security:Issuer"],
                      ValidAudience = builder.Configuration["Security:Audience"],

                      IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Security:SecretKey"]))
                  };
              });

            // Add Authorization Policies for Permissions Dynamical From AllPermissions Class
            builder.Services.AddAuthorization(options =>
            {
                foreach (var permission in permissions)
                {
                    options.AddPolicy(permission!, policy =>
                    {
                        policy.RequireClaim("permission", permission!);
                    });
                }
            });

            // Add EncryptService & HashService
            builder.Services.AddSingleton<IEncryptService, EncryptService>();
            builder.Services.AddSingleton<IHashService, HashService>();

            builder.Services.AddControllers();

            // Add Localization Service
            builder.Services.AddSingleton<ILocalizationService, JsonLocalizationService>();

            return builder;
        }
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

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseExceptionHandler();
            app.MapControllers();

            return app;
        }
        public static WebApplicationBuilder AddMongoDB(this WebApplicationBuilder builder, IMongoDbConfiguration mongoDbConfiguration)
        {
            var connectionString = builder.Configuration["MongoDB:ConnectionString"];
            // Register MongoClient as singleton
            builder.Services.AddSingleton<IMongoClient>(options =>
            {
                return new MongoClient(connectionString);
            });

            mongoDbConfiguration.Configure();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));


            return builder;
        }
        public static WebApplicationBuilder AddRedis(this WebApplicationBuilder builder)
        {
            // Add Redis Cache
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["RedisSettings:ConnectionString"];
            });

            builder.Services.AddScoped(typeof(ICacheRepository<>), typeof(RedisRepository<>));

            return builder;
        }
        public static WebApplicationBuilder AddSqlServer<TDbContext>(this WebApplicationBuilder builder, string connectionString)
        where TDbContext : DbContext
        {
            builder.Services.AddDbContext<TDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString);

                // Register the Auditing Interceptor with the DbContext
                options.AddInterceptors(sp.GetRequiredService<AuditingSaveChangesInterceptor>());

            });
            builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<TDbContext>());
            builder.Services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));
            builder.Services.AddScoped(typeof(ITransactionRepository<>), typeof(EFTransactionRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork<TDbContext>>();
            return builder;
        }
        public static WebApplicationBuilder AddPostgreSQL<TDbContext>(this WebApplicationBuilder builder)
            where TDbContext : DbContext
        {
            var connectionString = builder.Configuration["PostgresDB:ConnectionString"];
            builder.Services.AddDbContext<TDbContext>((sp, options) =>
            {
                options.UseNpgsql(connectionString);

                // Register the Auditing Interceptor with the DbContext
                options.AddInterceptors(sp.GetRequiredService<AuditingSaveChangesInterceptor>());
            });
            //builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<CatalogPostgresDbContext>());
            builder.Services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));
            builder.Services.AddScoped(typeof(ITransactionRepository<>), typeof(EFTransactionRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork<TDbContext>>();
            return builder;
        }
    }
}
