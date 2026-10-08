using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Platform.Lib.DTOs;
using Platform.Lib.Infrastructure.Authorization;
using Platform.Lib.Persistence.IRepositories;
using Platform.Lib.Services.Identity.Enums;
using Platform.Lib.Services.Security;

namespace FileStorage.API.Controllers
{

    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class CustomerFileController : ControllerBase
    {
        private readonly IRepository<Core.Persistence.Entities.File> _fileRepository;
        private readonly ICurrentUserService _currentUserService;

        public CustomerFileController(IRepository<Core.Persistence.Entities.File> fileRepository, ICurrentUserService currentUserService)
        {
            _fileRepository = fileRepository;
            _currentUserService = currentUserService;
        }

        [Authorize]
        [UserTypeAuthorize(UserTypes.Customer)]
        [HttpPost]
        public async Task<Result<string>> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return Result<string>.Failure("FileRequired");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "CustomerFiles", DateTime.UtcNow.Year.ToString(), DateTime.UtcNow.Month.ToString());

            Directory.CreateDirectory(path);

            var fileId = Guid.NewGuid();
            var fileType = Path.GetExtension(file.FileName);
            var fileName = $"{fileId}{fileType}";
            var filleFullPath = Path.Combine(path, fileName);

            await using var stream = System.IO.File.Create(filleFullPath);
            await file.CopyToAsync(stream);

            await _fileRepository.CreateAsync(new Core.Persistence.Entities.File
            {
                Id = fileId,
                Path = path,
                Type = fileType,
                IsCustomer = true,
                CustomerId = _currentUserService.UserId
            });

            return Result<string>.Success(fileId.ToString());
        }


        [Authorize]
        [UserTypeAuthorize(UserTypes.Customer)]
        [HttpPost]
        public async Task<Result<string>> Replace(IFormFile file, Guid fileId)
        {
            var newFileId = await Upload(file);
            await Delete(fileId);
            return Result<string>.Success(newFileId.ToString());

        }
        [Authorize]
        [UserTypeAuthorize(UserTypes.Customer)]
        [HttpGet("{fileId:guid}")]
        public async Task<IActionResult> Get(Guid fileId)
        {
            var file = await _fileRepository.GetByIdAsync(fileId);

            // Check if it is customer file and Customer Own the file
            if (file.IsCustomer == false || file.CustomerId != _currentUserService.UserId) return NotFound();

            // Check if Folder Present
            if (!Directory.Exists(file?.Path)) return NotFound();

            // Check if File Present
            var filePath = Directory.GetFiles(file.Path, $"{fileId}{file.Type}").FirstOrDefault();
            if (filePath == null) return NotFound();

            var contentType = GetContentType(filePath);

            return PhysicalFile(
                filePath,
                contentType,
                enableRangeProcessing: true);
        }

        [Authorize]
        [UserTypeAuthorize(UserTypes.Customer)]
        [HttpDelete("{fileId:guid}")]
        public async Task<Result<string>> Delete(Guid fileId)
        {
            var file = await _fileRepository.GetByIdAsync(fileId);

            // Check if it is customer file and Customer Own the file
            if (file.IsCustomer == false || file.CustomerId != _currentUserService.UserId)
                return Result<string>.Failure("FileNotFound");

            // Check if Folder Present
            if (!Directory.Exists(file?.Path))
                return Result<string>.Failure("FileNotFound");

            // Check if File Present
            var filePath = Directory.GetFiles(file.Path, $"{fileId}{file.Type}").FirstOrDefault();
            if (filePath == null) return
                    Result<string>.Failure("FileNotFound");

            System.IO.File.Delete(filePath);

            await _fileRepository.DeleteByIdAsync(fileId);

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