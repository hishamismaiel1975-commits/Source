using Core.Services.Discount.DTOs;

namespace Core.Services.Discount;

public interface IDiscountService
{
    Task<DiscountResponse> GetDiscountAsync(Guid productId);
}
