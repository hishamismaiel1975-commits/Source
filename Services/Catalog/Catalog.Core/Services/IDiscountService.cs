using Catalog.Core.DTOs;

namespace Catalog.Core.Services;

public interface IDiscountService
{
    Task<DiscountDTO> GetDiscountAsync(Guid productId);
}
