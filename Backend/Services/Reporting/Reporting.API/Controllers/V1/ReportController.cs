using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Reporting.API.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ReportController : ControllerBase
    {
        public ReportController()
        {
        }


        [HttpGet("{reportId}")]
        //[Authorize]
        //[UserTypeAuthorize(UserTypes.Employee)]
        public IActionResult GetReport(Guid reportId)
        {
            // Implement your report generation logic here
            return Ok("Report generated successfully.");
        }
    }
}
