using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.MongoDB.Entities;
using Platform.Core.Models;
using Riok.Mapperly.Abstractions;


namespace Catalog.Application.Products.Mappers
{

    [Mapper]
    public static partial class ProductMapper
    {

        public static partial ProductResponse ToResponse(Product product);
        public static partial Pagination<ProductResponse> ToResponse(Pagination<Product> pagination);
        public static partial IList<ProductResponse> ToResponse(IEnumerable<Product> products);

        [MapperIgnoreSource(nameof(CreateProductCommand.BrandId))]
        [MapperIgnoreSource(nameof(CreateProductCommand.TypeId))]
        [MapperIgnoreTarget(nameof(Product.Id))]
        public static partial Product ToEntity(CreateProductCommand command, ProductBrand brand, ProductType type, DateTimeOffset CreatedDate);

        [MapperIgnoreSource(nameof(UpdateProductCommand.BrandId))]
        [MapperIgnoreSource(nameof(UpdateProductCommand.TypeId))]
        public static partial Product ToEntity(UpdateProductCommand command, ProductBrand brand, ProductType type, DateTimeOffset CreatedDate);

    }
}
