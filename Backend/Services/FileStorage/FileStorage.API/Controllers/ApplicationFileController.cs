using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Lib.Constants;
using Platform.Lib.DTOs;
using Platform.Lib.Exceptions;
using Platform.Lib.Infrastructure.Authorization;
using Platform.Lib.Persistence.IRepositories;
using Platform.Lib.Services.File;
using Platform.Lib.Services.Grpc.Identity.Enums;

namespace FileStorage.API.Controllers
{

    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class ApplicationFileController : ControllerBase
    {
        private readonly IRepository<Platform.Lib.Persistence.Entities.File> _fileRepository;
        private readonly IFileStorageService _fileStorageService;

        public ApplicationFileController(IRepository<Platform.Lib.Persistence.Entities.File> fileRepository, IFileStorageService fileStorageService)
        {
            _fileRepository = fileRepository;
            _fileStorageService = fileStorageService;
        }

        [Authorize]
        [Authorize(Policy = PermissionConstants.FileStorage.Upload)]
        [UserTypeAuthorize(UserTypes.Employee)]
        [HttpPost]
        public async Task<Result<string>> Upload(IFormFile file)
        {
            var fileId = await _fileStorageService.UploadAsync(file, "ApplicationFiles", _fileRepository);
            return Result<string>.Success(fileId.ToString());
        }

        [Authorize]
        [Authorize(Policy = PermissionConstants.FileStorage.Upload)]
        [UserTypeAuthorize(UserTypes.Employee)]
        [HttpPost("Replace/{fileId:guid}")]
        public async Task<Result<string>> Replace(IFormFile file, Guid fileId)
        {
            var newFileId = await _fileStorageService.ReplaceAsync(file, fileId, "ApplicationFiles", _fileRepository);
            return Result<string>.Success(newFileId.ToString());
        }

        [Authorize]
        [Authorize(Policy = PermissionConstants.FileStorage.Upload)]
        [UserTypeAuthorize(UserTypes.Employee)]
        [HttpPost("ReplaceCustomerFile/{fileId:guid}")]
        public async Task<Result<string>> ReplaceCustomerFile(IFormFile file, Guid fileId)
        {
            var oldFile = await _fileRepository.GetByIdAsync(fileId);
            if (oldFile == null || oldFile.IsCustomer == false || oldFile.CustomerId == null) AppException.Throw("NotFound");

            var newFileId = await _fileStorageService.ReplaceAsync(file, fileId, "CustomerFiles", _fileRepository, true, oldFile.CustomerId);
            return Result<string>.Success(newFileId.ToString());
        }

        [Authorize]
        [Authorize(Policy = PermissionConstants.FileStorage.Read)]
        [UserTypeAuthorize(UserTypes.Employee)]
        [HttpGet("{fileId:guid}")]
        public async Task<IActionResult> Get(Guid fileId)
        {
            var result = await _fileStorageService.GetAsync(fileId, _fileRepository);
            return File(
                    result.Content,
                    result.ContentType,
                    enableRangeProcessing: true);
        }

        [Authorize]
        [Authorize(Policy = PermissionConstants.FileStorage.Delete)]
        [UserTypeAuthorize(UserTypes.Employee)]
        [HttpDelete("{fileId:guid}")]
        public async Task Delete(Guid fileId)
        {
            await _fileStorageService.DeleteAsync(fileId, _fileRepository);
        }



    }
}