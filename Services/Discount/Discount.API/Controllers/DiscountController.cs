using Microsoft.AspNetCore.Mvc;
using Platform.Core.DTOs.Discount;

namespace Discount.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DiscountController : ControllerBase
    {
        [HttpGet]
        public DiscountResponse GetDiscount(Guid productId)
        {
            var response = new DiscountResponse(productId, 10);
            return response;
        }
    }
}
