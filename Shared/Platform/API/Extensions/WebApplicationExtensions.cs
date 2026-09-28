using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using FreeMediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Driver;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Platform.Lib.API.Exceptions;
using Platform.Lib.API.Swagger;
using Platform.Lib.Application.Behaviors;
using Platform.Lib.Core.Persistence.MongoDB;
using Platform.Lib.Core.Persistence.Repositories;
using Platform.Lib.Core.Services.Identity;
using Platform.Lib.Core.Services.Identity.Enums;
using Platform.Lib.Core.Services.Localization;
using Platform.Lib.Core.Services.Security;
using Platform.Lib.Core.Services.Token;
using Platform.Lib.Infrastructure.Authorization;
using Platform.Lib.Infrastructure.Persistence.EFCore.Interceptors;
using Platform.Lib.Infrastructure.Persistence.EFCore.Repositories;
using Platform.Lib.Infrastructure.Persistence.MongoDB.Repositories;
using Platform.Lib.Infrastructure.Services.Identity;
using Platform.Lib.Infrastructure.Services.Localization;
using Platform.Lib.Infrastructure.Services.Security;
using Platform.Lib.Infrastructure.Services.Token;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

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

            // Register Auditing 
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<AuditingSaveChangesInterceptor>();

            // CurrentUser services & TokenService
            builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
            builder.Services.AddSingleton<ITokenService, TokenService>();

            // Add Authorization Services
            builder.Services.AddSingleton<IAuthorizationHandler, UserTypeAuthorizationHandler>();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, UserTypePolicyProvider>();

            // Add EncryptService & HashService
            builder.Services.AddSingleton<IEncryptService, EncryptService>();
            builder.Services.AddSingleton<IHashService, HashService>();

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

                var apiName = typeof(TProgram).Assembly.GetName().Name!.Split('.')[0];
                options.AddServer(new OpenApiServer
                {
                    Url = $"/{apiName}"
                });
            });


            //Register FreeMediator 
            builder.Services.AddMediator(config =>
            {
                var assemblies = new Assembly[] { Assembly.GetExecutingAssembly(), typeof(TMediatr).Assembly };
                config.RegisterServicesFromAssemblies(assemblies);
            });
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.AddValidatorsFromAssemblyContaining<TMediatr>();




            // Add IdentityService Service
            builder.Services.AddSingleton<IIdentityService, IdentityGrpcClient>();

            // Add JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(options =>
              {
                  options.MapInboundClaims = false;
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

                  // Add gRPC call to fetch user permissions and add them as claims
                  options.Events = new JwtBearerEvents
                  {
                      OnTokenValidated = async context =>
                      {
                          // Check if the token type is "access_token"
                          var tokenType = context.Principal?.FindFirst("token_type")?.Value;
                          if (!string.Equals(
                                  tokenType,
                                  "access_token",
                                  StringComparison.OrdinalIgnoreCase))
                          {
                              context.Fail("Invalid token type.");
                          }

                          var userId = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

                          if (!Guid.TryParse(userId, out var userGuidId))
                          {
                              context.Fail("Invalid user ID.");
                              return;
                          }

                          var identityGrpcClient = context.HttpContext.RequestServices.GetRequiredService<IIdentityService>();
                          var response = await identityGrpcClient.GetUserInfoAsync(userGuidId);
                          if (!response.IsActive)
                          {
                              context.Fail("User account is locked or inactive.");
                              return;
                          }

                          var identity = context.Principal!.Identity as ClaimsIdentity;

                          identity!.AddClaim(new Claim("name_en", response.NameEn));
                          identity!.AddClaim(new Claim("name_ar", response.NameAr));
                          identity!.AddClaim(new Claim("usertype", response.UserType.ToString().ToLower()));

                          // Add permissions as claims if the user is an employee
                          if (response.UserType == UserTypes.Employee.ToString().ToLower())
                          {
                              foreach (var permission in response.Permissions)
                              {
                                  identity!.AddClaim(
                                      new Claim("permission", permission));
                              }
                          }
                      }
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


            // Json Naming CamelCase for all Response to be like client side 
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy =
                            JsonNamingPolicy.CamelCase;
                    });

            // Add Localization Service
            builder.Services.AddSingleton<ILocalizationService, JsonLocalizationService>();

            return builder;
        }

        public static WebApplicationBuilder AddGrafanaOTEL(this WebApplicationBuilder builder, string ServiceName)
        {
            //Add OpenTelemetry services & Add Grafana OTEL
            var endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(ServiceName);

            // =============================
            // Logging
            // =============================
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Logging.AddOpenTelemetry(options =>
            {
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;

                options.SetResourceBuilder(resourceBuilder);
                options.AddOtlpExporter(exporter =>
                {
                    exporter.Endpoint = new Uri(endpoint + "/v1/logs");


                    exporter.Protocol =
                        OtlpExportProtocol.HttpProtobuf;
                });
            });

            // =============================
            // OpenTelemetry
            // =============================
            builder.Services.AddOpenTelemetry()

             // =========================
             // Tracing
             // =========================
             .WithTracing(tracing =>
             {
                 tracing
                     .SetResourceBuilder(resourceBuilder)
                     .SetSampler(new AlwaysOnSampler())
                     .AddAspNetCoreInstrumentation()
                     .AddOtlpExporter(exporter =>
                     {
                         exporter.Endpoint = new Uri(endpoint + "/v1/traces");


                         exporter.Protocol =
                             OtlpExportProtocol.HttpProtobuf;
                     });
             })

                // =========================
                // Metrics
                // =========================
                .WithMetrics(metrics =>
                {
                    metrics.SetResourceBuilder(resourceBuilder);
                    metrics.AddAspNetCoreInstrumentation();
                    metrics.AddRuntimeInstrumentation();
                    metrics.AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = new Uri(endpoint + "/v1/metrics");


                        exporter.Protocol =
                            OtlpExportProtocol.HttpProtobuf;
                    });
                });

            return builder;
        }

        public static WebApplication UsePlatform<TProgram>(this WebApplication app)
        {
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
                            $"/{apiName.Split('.')[0]}/swagger/{description.GroupName}/swagger.json",
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
                options.UseSqlServer(connectionString,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });


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
