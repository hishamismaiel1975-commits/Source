namespace Catalog.Application.Products.Responses
{
    public record ProductResponse
   (
        Guid Id,
        string Name,
        string Summary,
        string Description,
        string ImageFile,
        Guid ProductBrandId,
        Guid ProductTypeId,
        decimal Price,
        DateTime CreatedDate,

        ProductBrandResponse? ProductBrand,
        ProductTypeResponse? ProductType

    );
}
