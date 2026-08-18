using Catalog.Core.Persistence.Entities;

namespace Catalog.Application.Products.Responses
{
    public record ProductResponse
   (
        Guid Id,
        string Name,
        string Summary,
        string Description,
        string ImageFile,
        Guid BrandId,
        Guid TypeId,
        decimal Price,
        DateTime CreatedDate,

        ProductBrand? ProductBrand,
        ProductType? ProductType

    );
}
