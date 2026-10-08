using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Platform.Lib.DTOs;
using Platform.Lib.Infrastructure.Authorization;
using Platform.Lib.Services.Identity.Enums;

namespace FileStorage.API.Controllers
{

    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class PublicFileController : ControllerBase
    {
        [Authorize]
        [UserTypeAuthorize(UserTypes.Customer)]
        [HttpPost]
        public async Task<Result<string>> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return Result<string>.Failure("FileRequired");

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            Directory.CreateDirectory(folder);

            var fileId = Guid.NewGuid();
            var fileName = $"{fileId}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);

            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            return Result<string>.Success(fileId.ToString());
        }

        [Authorize]
        [UserTypeAuthorize(UserTypes.Customer)]
        [HttpGet("{fileId:guid}")]
        public IActionResult Get(Guid fileId)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(folder))
                return NotFound();

            var filePath = Directory
                .GetFiles(folder, $"{fileId}.*")
                .FirstOrDefault();

            if (filePath == null)
                return NotFound();

            var contentType = GetContentType(filePath);

            return PhysicalFile(
                filePath,
                contentType,
                enableRangeProcessing: true);
        }

        [Authorize]
        [UserTypeAuthorize(UserTypes.Customer)]
        [HttpDelete("{fileId:guid}")]
        public Result<string> Delete(Guid fileId)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(folder))
                return Result<string>.Failure("FolderNotFound");

            var filePath = Directory
                .GetFiles(folder, $"{fileId}.*")
                .FirstOrDefault();

            if (filePath == null)
                return Result<string>.Failure("FileNotFound");

            System.IO.File.Delete(filePath);

            return Result<string>.Success(fileId.ToString());
        }

        private static string GetContentType(string filePath)
        {
            return new FileExtensionContentTypeProvider()
                .TryGetContentType(filePath, out var contentType)
                    ? contentType
                    : "application/octet-stream";
        }

    }
}