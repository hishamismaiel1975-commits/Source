using Application.Lib.Core.Services.Discount;
using Application.Lib.Core.Services.Identity.DTOs;
using System.Net.Http.Json;

namespace Application.Lib.Infrastructure.Services.Discount.HttpClients
{
    public class DiscountHttpClient : IIdentityService
    {
        private readonly HttpClient _httpClient;
        public DiscountHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<UserPermissionsResponse> GetDiscountAsync(Guid productId)
        {
            return await _httpClient.GetFromJsonAsync<UserPermissionsResponse>($"api/discount/{productId}");
        }


    }


}
