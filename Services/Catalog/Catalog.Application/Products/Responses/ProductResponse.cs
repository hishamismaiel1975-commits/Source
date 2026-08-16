using Catalog.Core.Persistence.MongoDB.Entities;

namespace Catalog.Application.Products.Responses
{
    public record ProductResponse
   (
        string Id,
        string Name,
        string Summary,
        string Description,
        string ImageFile,
        Brand Brand,
        Core.Persistence.MongoDB.Entities.Type Type,
        decimal Price,
        DateTimeOffset CreatedDate
    );
}
