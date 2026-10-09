using Platform.Lib.Services.Grpc.Catalog.DTOs;

namespace Platform.Lib.Services.Grpc.Catalog;

public interface ICatalogService
{
    Task<ProductsDTO[]> GetProductsAsync(Guid? brandId = null, Guid? typeId = null);
}
