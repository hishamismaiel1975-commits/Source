using Application.Lib.Core.Services.Discount.DTOs;

namespace Application.Lib.Core.Services.Discount;

public interface IDiscountService
{
    Task<DiscountResponse> GetDiscountAsync(Guid productId);
}
