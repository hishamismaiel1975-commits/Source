using Microsoft.AspNetCore.Http;
using Platform.Lib.Persistence.IRepositories;

namespace Platform.Lib.Services.File
{

    public interface IFileStorageService
    {
        Task<Guid> UploadAsync(IFormFile file, string folderName, IRepository<Persistence.Entities.File> fileRepository, bool isCustomer = false, Guid? customerId = null);
        Task<Guid> ReplaceAsync(IFormFile file, Guid fileId, string folderName, IRepository<Persistence.Entities.File> fileRepository, bool isCustomer = false, Guid? customerId = null);
        Task<FileDownloadResult> GetAsync(Guid fileId, IRepository<Persistence.Entities.File> fileRepository);
        Task DeleteAsync(Guid fileId, IRepository<Persistence.Entities.File> fileRepository);

    }
}
