using Discount.GRPC;
using Infrastructure.Grpc.Discount;
using Platform.Lib.Core;
using Platform.Lib.Core.Services;

namespace Platform.Lib.Infrastructure.Services.Discount.Grpc
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
