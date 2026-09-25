using Microsoft.AspNetCore.Server.Kestrel.Core;
using Platform.Lib.API.Extensions;

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

builder.AddGrafanaOTEL("Gateway-api");

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
