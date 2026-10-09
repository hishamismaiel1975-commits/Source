namespace Platform.Lib.Services.Grpc.Catalog.DTOs
{
    public record ProductsDTO
    (
         string Name,
         string Description,
         decimal Price
    );
}
