using Discount.GRPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Platform.Core.Services;

namespace Infrastructure.Grpc.Discount;

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
