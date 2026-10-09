using Catalog.GRPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Lib.Services.Grpc.Catalog;

public static class CatalogGrpcClientConfig
{
    public static WebApplicationBuilder AddCatalogGrpcClientService(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpcClient<CatalogService.CatalogServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GrpcSettings:CatalogServiceUrl"]!);
        });

        builder.Services.AddScoped<ICatalogService, CatalogGrpcClient>();

        return builder;
    }

}
