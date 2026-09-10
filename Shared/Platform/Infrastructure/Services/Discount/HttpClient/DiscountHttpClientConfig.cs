using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Platform.Lib.Core.Services;

namespace Platform.Lib.Infrastructure.Services.Discount.HttpClients;

public static class DiscountHttpClientConfig
{
    public static WebApplicationBuilder AddDiscountHttpClientService(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<IDiscountService, DiscountHttpClient>(client =>
        {
            client.BaseAddress = new Uri("http://discount.api:80/");
        });

        return builder;
    }

}
