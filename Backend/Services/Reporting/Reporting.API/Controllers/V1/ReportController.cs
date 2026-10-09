using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Platform.Lib.Services.Grpc.Catalog;

namespace Reporting.API.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public ReportController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }


        [HttpGet("{reportId}")]
        //[Authorize]
        //[UserTypeAuthorize(UserTypes.Employee)]
        public async Task<IActionResult> GetReport(Guid reportId, [FromQuery] Guid? brandId, [FromQuery] Guid? typeId)
        {
            var products = await _catalogService.GetProductsAsync(brandId, typeId);

            // Implement your report generation logic here
            return Ok("Report generated successfully.");
        }
    }
}
