using Microsoft.AspNetCore.Http;
using Platform.Lib.Persistence.IRepositories;

namespace Platform.Lib.Services.File;

public sealed class AzureBlobFileService : IFileStorageService
{
    public AzureBlobFileService()
    {
    }

    public Task DeleteAsync(Guid fileId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid fileId, IRepository<Persistence.Entities.File> fileRepository)
    {
        throw new NotImplementedException();
    }

    public Task<FileDownloadResult> GetAsync(Guid fileId, IRepository<Persistence.Entities.File> fileRepository)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> ReplaceAsync(IFormFile file, Guid fileId, string folderName, IRepository<Persistence.Entities.File> fileRepository, bool isCustomer = false, Guid? customerId = null)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> UploadAsync(IFormFile file, string folderName, IRepository<Persistence.Entities.File> fileRepository, bool isCustomer = false, Guid? customerId = null)
    {
        throw new NotImplementedException();
    }
}