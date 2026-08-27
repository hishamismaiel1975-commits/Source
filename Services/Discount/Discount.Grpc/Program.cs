using Discount.Grpc.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
//For postman support
builder.Services.AddGrpcReflection();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

var app = builder.Build();

// Add gRPC service to the request pipeline.
app.MapGrpcService<DiscountGrpcService>();
//For postman support
app.MapGrpcReflectionService();



app.MapGet("/", () =>
    "This is a gRPC service. Use a gRPC client to communicate with it.");

app.Run();
