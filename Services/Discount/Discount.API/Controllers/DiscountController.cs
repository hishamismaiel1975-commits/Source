using Core.Services.Discount.DTOs;
using Microsoft.AspNetCore.Mvc;

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
