using Application.Lib.Core.DTOs.Discount;

namespace Application.Lib.Core.Services.Discount;

public interface IDiscountService
{
    Task<DiscountResponse> GetDiscountAsync(Guid productId);
}
