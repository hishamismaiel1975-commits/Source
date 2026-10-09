using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Platform.Lib.DTOs;
using Platform.Lib.Exceptions;
using Platform.Lib.Infrastructure.Authorization;
using Platform.Lib.Persistence.IRepositories;
using Platform.Lib.Services.File;
using Platform.Lib.Services.Grpc.Identity.Enums;
using Platform.Lib.Services.Security;

namespace FileStorage.API.Controllers
{

    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class CustomerFileController : ControllerBase
    {
        private readonly IRepository<Platform.Lib.Persistence.Entities.File> _fileRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICurrentUserService _currentUserService;


        public CustomerFileController(IRepository<Platform.Lib.Persistence.Entities.File> fileRepository, IFileStorageService fileStorageService, ICurrentUserService currentUserService)
        {
            _fileRepository = fileRepository;
            _fileStorageService = fileStorageService;
            _currentUserService = currentUserService;
        }

        [Authorize]
        [UserTypeAuthorize(UserTypes.Customer)]
        [HttpPost]
        public async Task<Result<string>> Upload(IFormFile file)
        {
            var fileId = await _fileStorageService.UploadAsync(file, "CustomerFiles", _fileRepository, isCustomer: true, customerId: _currentUserService.UserId);
            return Result<string>.Success(fileId.ToString());
        }

        [Authorize]
        [UserTypeAuthorize(UserTypes.Customer)]
        [HttpPost("Replace/{fileId:guid}")]
        public async Task<Result<string>> Replace(IFormFile file, Guid fileId)
        {
            var newFileId = await _fileStorageService.ReplaceAsync(file, fileId, "CustomerFiles", _fileRepository, isCustomer: true, customerId: _currentUserService.UserId);
            return Result<string>.Success(newFileId.ToString());
        }

        [Authorize]
        [UserTypeAuthorize(UserTypes.Customer)]
        [HttpGet("{fileId:guid}")]
        public async Task<IActionResult> Get(Guid fileId)
        {
            var file = await _fileRepository.GetByIdAsync(fileId);
            if (file == null || file.IsCustomer == false || file.CustomerId != _currentUserService.UserId) return NotFound();

            var result = await _fileStorageService.GetAsync(fileId, _fileRepository);
            return File(
                    result.Content,
                    result.ContentType,
                    enableRangeProcessing: true);
        }

        [Authorize]
        [UserTypeAuthorize(UserTypes.Customer)]
        [HttpDelete("{fileId:guid}")]
        public async Task Delete(Guid fileId)
        {
            var file = await _fileRepository.GetByIdAsync(fileId);
            if (file == null || file.IsCustomer == false || file.CustomerId != _currentUserService.UserId)
                AppException.Throw("FileNotFound");

            await _fileStorageService.DeleteAsync(fileId, _fileRepository);
        }


    }
}