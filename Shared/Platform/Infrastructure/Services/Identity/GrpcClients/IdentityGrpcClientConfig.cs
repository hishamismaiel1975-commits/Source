using Identity.GRPC;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Platform.Lib.Core.Services.Identity;

namespace Platform.Lib.Infrastructure.Services.Identity.GrpcClients;

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
