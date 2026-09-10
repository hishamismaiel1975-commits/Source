
using Discount.API.EventBus.Consumer;
using Discount.API.Grpc.Services;
using MassTransit;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on specific ports
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });

    // gRPC services require HTTP/2, so configure Kestrel to support it
    options.ListenAnyIP(81, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});


// Add gRPC & gRPC reflection for postman support
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Add MassTransit with RabbitMQ configuration
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<CreateProductConsumer>();
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

        // Configure the receive endpoint for the consumer
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Add gRPC service to the request pipeline & reflection for postman support
app.MapGrpcService<DiscountGrpcService>();
app.MapGrpcReflectionService();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
