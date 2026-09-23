namespace Catalog.Application.Products.Responses
{
    public record ProductResponse
   (
        Guid id,
        string name,
        string summary,
        string description,
        string imageFile,
        Guid productBrandId,
        Guid productTypeId,
        decimal price,
        DateTime createdDate,

        ProductBrandResponse? productBrand,
        ProductTypeResponse? productType

    );
}
