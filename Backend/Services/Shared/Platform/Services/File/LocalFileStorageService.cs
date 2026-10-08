using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Platform.Lib.Exceptions;
using Platform.Lib.Persistence.IRepositories;
using Platform.Lib.Services.Security;

namespace Platform.Lib.Services.File;

public class LocalFileStorageService : IFileStorageService
{
    private readonly ICurrentUserService _currentUserService;

    public LocalFileStorageService(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<Guid> UploadAsync(IFormFile file, string folderName, IRepository<Persistence.Entities.File> fileRepository, bool isCustomer = false, Guid? customerId = null)
    {
        if (file == null || file.Length == 0) AppException.Throw("FileRequired");

        var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", folderName, DateTime.UtcNow.Year.ToString(), DateTime.UtcNow.Month.ToString());

        Directory.CreateDirectory(path);

        var fileId = Guid.NewGuid();
        var fileType = Path.GetExtension(file.FileName);
        var fileName = $"{fileId}{fileType}";
        var filleFullPath = Path.Combine(path, fileName);

        await using var stream = System.IO.File.Create(filleFullPath);
        await file.CopyToAsync(stream);

        await fileRepository.CreateAsync(new Persistence.Entities.File
        {
            Id = fileId,
            Path = path,
            Type = fileType,
            IsCustomer = isCustomer,
            CustomerId = customerId
        });

        return fileId;
    }

    public async Task<Guid> ReplaceAsync(IFormFile file, Guid fileId, string folderName, IRepository<Persistence.Entities.File> fileRepository, bool isCustomer = false, Guid? customerId = null)
    {
        var newFileId = await UploadAsync(file, folderName, fileRepository, isCustomer, customerId);
        await DeleteAsync(fileId, fileRepository);
        return newFileId;
    }

    public async Task<FileDownloadResult> GetAsync(Guid fileId, IRepository<Persistence.Entities.File> fileRepository)
    {
        var file = await fileRepository.GetByIdAsync(fileId);
        if (file == null) AppException.Throw("FileNotFound");

        // Check if Folder Present
        if (!Directory.Exists(file?.Path)) AppException.Throw("FileNotFound");

        // Check if File Present
        var filePath = Directory.GetFiles(file.Path, $"{fileId}{file.Type}").FirstOrDefault();
        if (filePath == null) AppException.Throw("FileNotFound");

        var contentType = GetContentType(filePath);

        var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);

        return new FileDownloadResult(
            stream,
            contentType,
            fileId.ToString());
    }

    public async Task DeleteAsync(Guid fileId, IRepository<Persistence.Entities.File> fileRepository)
    {
        var file = await fileRepository.GetByIdAsync(fileId);
        if (file == null) AppException.Throw("FileNotFound");

        // Check if Folder Present
        if (!Directory.Exists(file?.Path))
            AppException.Throw("FileNotFound");

        // Check if File Present
        var filePath = Directory.GetFiles(file.Path, $"{fileId}{file.Type}").FirstOrDefault();
        if (filePath == null) AppException.Throw("FileNotFound");

        System.IO.File.Delete(filePath);

        await fileRepository.DeleteByIdAsync(fileId);
    }

    private static string GetContentType(string filePath)
    {
        return new FileExtensionContentTypeProvider()
            .TryGetContentType(filePath, out var contentType)
                ? contentType
                : "application/octet-stream";
    }


}

