using Discount.GRPC;
using Grpc.Core;

namespace Discount.API.Grpc.Services;

public class DiscountGrpcService : DiscountService.DiscountServiceBase
{
    public override Task<GetDiscountResponse> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var response = new GetDiscountResponse
        {
            ProductId = request.ProductId,
            Amount = 10
        };

        return Task.FromResult(response);
    }


}
