
using Discount.API.GrpcServices;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add gRPC & gRPC reflection for postman support
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

var app = builder.Build();

// Add gRPC service to the request pipeline & reflection for postman support
app.MapGrpcService<DiscountGrpcService>();
app.MapGrpcReflectionService();


app.MapGet("/", () =>
    "This is a gRPC service. Use a gRPC client to communicate with it.");

app.Run();
