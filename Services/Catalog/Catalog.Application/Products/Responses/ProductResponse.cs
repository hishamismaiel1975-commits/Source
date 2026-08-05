using Catalog.Core.Entities;

namespace Catalog.Application.Products.Responses
{
    public record ProductResponse
   (
        string Id,
        string Name,
        string Summary,
        string Description,
        string ImageFile,
        ProductBrand Brand,
        ProductType Type,
        decimal Price,
        DateTimeOffset CreatedDate
    );
}
