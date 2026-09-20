using Microsoft.AspNetCore.Server.Kestrel.Core;

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


var app = builder.Build();

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
