using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Lib.Constants;
using Platform.Lib.Infrastructure.Authorization;
using Platform.Lib.Persistence.IRepositories;
using Platform.Lib.Services.Grpc.Catalog;
using Platform.Lib.Services.Grpc.Identity.Enums;
using QuestPDF.Fluent;
using Reporting.API.Reports.Products;
using Reporting.Core.Persistence.Entities;

namespace Reporting.API.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IRepository<Report> _repository;
        private readonly ICatalogService _catalogService;
        public ReportController(ICatalogService catalogService, IRepository<Report> repository)
        {
            _catalogService = catalogService;
            _repository = repository;

        }


        [HttpGet("{reportId}")]
        [Authorize(Policy = PermissionConstants.Reporting.Read)]
        [UserTypeAuthorize(UserTypes.Employee)]
        public async Task<IActionResult> GetReport(Guid reportId, [FromQuery] Guid? brandId, [FromQuery] Guid? typeId)
        {
            switch (reportId.ToString())
            {
                //Products Report
                case "4548273b-2d43-42d4-9a57-5874668df4fc":
                    var report = await _repository.GetByIdAsync(Guid.Parse("4548273b-2d43-42d4-9a57-5874668df4fc"));
                    if (report == null) return NotFound();

                    var products = await _catalogService.GetProductsAsync(brandId, typeId);
                    var pdf = new ProductsReport(products, report.Title).GeneratePdf();
                    return File(pdf, "application/pdf");

                default:
                    return NotFound();
            }

        }
    }
}
