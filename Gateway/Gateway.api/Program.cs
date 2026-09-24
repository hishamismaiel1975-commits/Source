using Microsoft.AspNetCore.Server.Kestrel.Core;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    // Public HTTP API
    options.ListenAnyIP(80, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });

    // Internal gRPC
    options.ListenAnyIP(81, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });

    // Internal/Admin HTTP
    options.ListenAnyIP(8000, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });

});

// Load Yarb reverseproxy setting file
builder.Configuration
    .AddJsonFile(
        "reverseproxy.json",
        optional: false,
        reloadOnChange: true);

// Add services to the container.
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add Cors
var corsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("GatewayCors", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

//Add OpenTelemetry services & Add Grafana OTEL
var endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(builder.Configuration["OTEL_Service_Name"]!);

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

var app = builder.Build();

app.UseCors("GatewayCors");

app.Use(async (context, next) =>
{
    var port = context.Connection.LocalPort;

    var isAdminRoute =
        context.Request.Path.StartsWithSegments("/portainer") ||
        context.Request.Path.StartsWithSegments("/rabbitmq") ||
        context.Request.Path.StartsWithSegments("/redisinsight");

    if (isAdminRoute && port != 8000)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    await next();
});

app.MapReverseProxy();

app.Run();
