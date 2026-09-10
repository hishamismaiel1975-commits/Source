using Platform.Lib.Core;
using Platform.Lib.Core.Services;
using System.Net.Http.Json;

namespace Platform.Lib.Infrastructure.Services.Discount.HttpClients
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
