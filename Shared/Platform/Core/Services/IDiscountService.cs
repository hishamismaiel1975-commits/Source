namespace Platform.Lib.Core.Services;

public interface IDiscountService
{
    Task<DiscountResponse> GetDiscountAsync(Guid productId);
}
