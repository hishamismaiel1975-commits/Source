using Platform.Core.DTOs.Discount;
using Platform.Core.Services;
using System.Net.Http.Json;

namespace Platform.Infrastructure.Services.HttpClients.Discount
{
    public class DiscountHttpClient : IDiscountService
    {
        private readonly HttpClient _httpClient;
        public DiscountHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<DiscountResponse> GetDiscountAsync(Guid productId)
        {
            return await _httpClient.GetFromJsonAsync<DiscountResponse>($"api/discount/{productId}");
        }
    }


}
