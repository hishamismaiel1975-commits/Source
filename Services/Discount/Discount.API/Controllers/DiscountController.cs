using Discount.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Discount.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscountController : ControllerBase
    {
        [HttpGet("{productId}")]
        public DiscountResponse GetDiscount(Guid productId)
        {
            var response = new DiscountResponse(productId, 10);
            return response;
        }
    }
}
