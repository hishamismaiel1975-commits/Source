using Application.Lib.Core.Services.Discount;
using Discount.GRPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Lib.Infrastructure.Services.Discount.GrpcClients;

public static class DiscountGrpcClientConfig
{
    public static WebApplicationBuilder AddDiscountGrpcService(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpcClient<DiscountService.DiscountServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountServiceUrl"]!);
        });

        builder.Services.AddScoped<IDiscountService, DiscountGrpcClient>();

        return builder;
    }

}
