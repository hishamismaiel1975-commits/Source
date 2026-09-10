using Discount.GRPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Platform.Lib.Core.Services;

namespace Platform.Lib.Infrastructure.Services.Discount.Grpc;

public static class DiscountGrpcConfig
{
    public static WebApplicationBuilder AddDiscountGrpcService(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpcClient<DiscountService.DiscountServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
        });

        builder.Services.AddScoped<IDiscountService, DiscountGrpcClient>();

        return builder;
    }

}
