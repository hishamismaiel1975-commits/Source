using Application.Lib.Core.Services.Discount;
using Application.Lib.Core.Services.Discount.DTOs;
using Application.Lib.Infrastructure.Services.Discount.Grpc;
using Discount.GRPC;

namespace Application.Lib.Infrastructure.Services.Discount.GrpcClients
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
