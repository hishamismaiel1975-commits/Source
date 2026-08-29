using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.Entities;
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
        private static DateTime ToSaudiTime(DateTime utcDate)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.SpecifyKind(utcDate, DateTimeKind.Utc),
                TimeZoneInfo.FindSystemTimeZoneById("Asia/Riyadh"));
        }

    }

    [Mapper]
    public static partial class ProductMapper
    {

        [MapperIgnoreTarget(nameof(Product.Id))]
        [MapperIgnoreTarget(nameof(Product.ProductBrand))]
        [MapperIgnoreTarget(nameof(Product.ProductType))]
        public static partial Product ToEntity(CreateProductCommand command, DateTime createdDate);

        [MapperIgnoreTarget(nameof(Product.ProductBrand))]
        [MapperIgnoreTarget(nameof(Product.ProductType))]
        public static partial Product ToEntity(UpdateProductCommand command, DateTime createdDate);
    }
}
