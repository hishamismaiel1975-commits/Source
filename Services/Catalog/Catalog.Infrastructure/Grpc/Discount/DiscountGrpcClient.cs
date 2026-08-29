using Catalog.Application.Types.Mappers;
using Catalog.Core.DTOs;
using Catalog.Core.Services;
using Discount.GRPC;

namespace Catalog.Infrastructure.Grpc.Discount
{
    public class DiscountGrpcClient : IDiscountService
    {
        public DiscountService.DiscountServiceClient _discountClient { get; set; }
        public DiscountGrpcClient(DiscountService.DiscountServiceClient discountClient)
        {
            _discountClient = discountClient;
        }

        public async Task<DiscountDTO> GetDiscountAsync(Guid productId)
        {
            var response = await _discountClient.GetDiscountAsync(new GetDiscountRequest { ProductId = productId.ToString() });
            return DiscountMapper.ToDTO(response);
        }
    }
}
