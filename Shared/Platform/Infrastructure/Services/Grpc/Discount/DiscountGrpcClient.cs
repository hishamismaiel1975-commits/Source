using Discount.GRPC;
using Platform.Core.DTOs.Discount;
using Platform.Core.Services;

namespace Infrastructure.Grpc.Discount
{
    public class DiscountGrpcClient : IDiscountService
    {
        public DiscountService.DiscountServiceClient _discountClient { get; set; }
        public DiscountGrpcClient(DiscountService.DiscountServiceClient discountClient)
        {
            _discountClient = discountClient;
        }

        public async Task<DiscountResponse> GetDiscountAsync(Guid productId)
        {
            var response = await _discountClient.GetDiscountAsync(new GetDiscountRequest { ProductId = productId.ToString() });
            return DiscountGrpcMapper.ToDTO(response);
        }
    }
}
