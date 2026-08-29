using Catalog.Core.Services;
using Catalog.Infrastructure.Grpc.Discount;
using Discount.GRPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure.Grpc;

public static class GrpcConfiguration
{
    public static WebApplicationBuilder AddGrpcServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpcClient<DiscountService.DiscountServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
        });

        builder.Services.AddScoped<IDiscountService, DiscountGrpcClient>();

        return builder;
    }

}
