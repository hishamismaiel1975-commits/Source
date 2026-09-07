
using Discount.API.EventBusConsumer;
using Discount.API.GrpcServices;
using EventBus.Messages.Constants;
using MassTransit;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add gRPC & gRPC reflection for postman support
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(81, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

// Add MassTransit with RabbitMQ configuration
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<CreateProductConsumer>();
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

        // Configure the receive endpoint for the consumer
        cfg.ReceiveEndpoint(EventBusConstants.CatalogDiscountQueue, c =>
        {
            c.ConfigureConsumer<CreateProductConsumer>(context);
        });
    });
});

var app = builder.Build();

// Add gRPC service to the request pipeline & reflection for postman support
app.MapGrpcService<DiscountGrpcService>();
app.MapGrpcReflectionService();

app.Run();
