using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Responses;
using Catalog.Core.Persistence.Entities;
using Platform.Lib.Core.DTOs;
using Platform.Lib.Core.Extensions;
using Riok.Mapperly.Abstractions;


namespace Catalog.Application.Products.Mappers
{

    [Mapper]
    public static partial class ProductResponseMapper
    {
        [MapperIgnoreSource(nameof(Product.CreatedBy))]
        [MapperIgnoreSource(nameof(Product.UpdatedBy))]
        [MapperIgnoreSource(nameof(Product.UpdatedDate))]
        public static partial ProductResponse ToResponse(Product product);
        public static partial Pagination<ProductResponse> ToResponse(Pagination<Product> pagination);
        public static partial IList<ProductResponse> ToResponse(IEnumerable<Product> products);

        // Auto Used By Mapper 
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
        [MapperIgnoreTarget(nameof(Product.CreatedBy))]
        [MapperIgnoreTarget(nameof(Product.UpdatedBy))]
        [MapperIgnoreTarget(nameof(Product.UpdatedDate))]
        public static partial Product ToEntity(CreateProductCommand command);

        [MapperIgnoreTarget(nameof(Product.ProductBrand))]
        [MapperIgnoreTarget(nameof(Product.ProductType))]
        [MapperIgnoreTarget(nameof(Product.CreatedDate))]
        [MapperIgnoreTarget(nameof(Product.CreatedBy))]
        [MapperIgnoreTarget(nameof(Product.UpdatedBy))]
        [MapperIgnoreTarget(nameof(Product.UpdatedDate))]
        public static partial Product ToEntity(UpdateProductCommand command);

        // Auto Used By Mapper 
        private static DateTime FromSaudiTimeToUtc(DateTime saudiTime)
        {
            return saudiTime.FromSaudiTimeToUtc();
        }
    }
}
