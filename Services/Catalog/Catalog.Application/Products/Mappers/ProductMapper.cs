using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.Entities;
using Platform.Core.Extensions;
using Platform.Core.Models;
using Riok.Mapperly.Abstractions;


namespace Catalog.Application.Products.Mappers
{

    [Mapper]
    public static partial class ProductResponseMapper
    {
        public static partial ProductResponse ToResponse(Product product);
        public static partial Pagination<ProductResponse> ToResponse(Pagination<Product> pagination);
        public static partial IList<ProductResponse> ToResponse(IEnumerable<Product> products);

        private static DateTime FromUtcToSaudiTime(DateTime utcTime)
        {
            return utcTime.FromUtcToSaudiTime();
        }
    }

    [Mapper]
    public static partial class ProductEntityMapper
    {
        [MapperIgnoreTarget(nameof(Product.Id))]
        [MapperIgnoreTarget(nameof(Product.ProductBrand))]
        [MapperIgnoreTarget(nameof(Product.ProductType))]
        [MapperIgnoreTarget(nameof(Product.CreatedDate))]
        public static partial Product ToEntity(CreateProductCommand command);

        [MapperIgnoreTarget(nameof(Product.ProductBrand))]
        [MapperIgnoreTarget(nameof(Product.ProductType))]
        [MapperIgnoreTarget(nameof(Product.CreatedDate))]
        public static partial Product ToEntity(UpdateProductCommand command);

        private static DateTime FromSaudiTimeToUtc(DateTime saudiTime)
        {
            return saudiTime.FromSaudiTimeToUtc();
        }
    }
}
