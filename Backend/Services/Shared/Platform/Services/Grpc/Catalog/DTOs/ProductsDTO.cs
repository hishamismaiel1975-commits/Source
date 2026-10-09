namespace Platform.Lib.Services.Grpc.Catalog.DTOs
{
    public record ProductsDTO
    (
    string Name,
    string Summary,
    string Description,
    string BrandName,
    string TypeName,
    string Price,
    string CreatedDate
    );
}
