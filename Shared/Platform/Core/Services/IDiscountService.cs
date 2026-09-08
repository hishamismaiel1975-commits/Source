using Platform.Core.DTOs.Discount;

namespace Platform.Core.Services;

public interface IDiscountService
{
    Task<DiscountResponse> GetDiscountAsync(Guid productId);
}
