using Application.Lib.Core.Services.Discount;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Lib.Infrastructure.Services.Discount.HttpClients
{
    public static class DiscountHttpClientConfig
    {
        public static WebApplicationBuilder AddDiscountHttpClientService(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddHttpClient<IDiscountService, DiscountHttpClient>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["HttpClient: DiscountUrl"]);
            });

            return builder;
        }
    }
}
