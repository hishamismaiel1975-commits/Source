using Core.Services.Discount;
using Discount.GRPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Grpc.Discount;

public static class GrpcConfiguration
{
    public static WebApplicationBuilder AddDiscountGrpcServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpcClient<DiscountService.DiscountServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
        });

        builder.Services.AddScoped<IDiscountService, DiscountGrpcClient>();

        return builder;
    }

}
