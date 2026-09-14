namespace Platform.Lib.Core.Services.Discount;

public interface IDiscountService
{
    Task<DiscountResponse> GetDiscountAsync(Guid productId);
}
