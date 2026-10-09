using Catalog.GRPC;

namespace Platform.Lib.Services.Grpc.Identity;

public interface ICatalogService
{
    Task<GetProductsResult> GetProductsAsync(Guid? brandId, Guid? typeId);
}
