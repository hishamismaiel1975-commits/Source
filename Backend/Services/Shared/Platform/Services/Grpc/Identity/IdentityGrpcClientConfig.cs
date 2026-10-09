using Identity.GRPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Lib.Services.Grpc.Identity;

public static class IdentityGrpcClientConfig
{
    public static WebApplicationBuilder AddIdentityGrpcClientService(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpcClient<IdentityService.IdentityServiceClient>(options =>
        {
            options.Address = new Uri(builder.Configuration["GrpcSettings:IdentityServiceUrl"]!);
        });

        builder.Services.AddScoped<IIdentityService, IdentityGrpcClient>();

        return builder;
    }

}
