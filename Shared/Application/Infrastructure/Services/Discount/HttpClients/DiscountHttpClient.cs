using Application.Lib.Core.Services.Discount;
using Application.Lib.Core.Services.Discount.DTOs;
using System.Net.Http.Json;

namespace Application.Lib.Infrastructure.Services.Discount.HttpClients
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
