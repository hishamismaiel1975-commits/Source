using Application.Lib.Core.Services.Discount;
using Identity.GRPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Lib.Infrastructure.Services.Identity.GrpcClients;

public static class IdentityGrpcClientConfig
{
    public static WebApplicationBuilder AddIdentityGrpcService(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpcClient<IdentityService.IdentityServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GrpcSettings:IdentityServiceUrl"]!);
        });

        builder.Services.AddScoped<IIdentityService, IdentityGrpcClient>();

        return builder;
    }

}
