using Catalog.Core.Persistence.MongoDB.Entities;
using Microsoft.Extensions.Options;
using Platform.Infrastructure.Persistence.MongoDB.Settings;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(IOptions<DatabaseSettings> options)
        {
            var _brandRepository = new Repositories.BrandRepository(options);
            var _typeRepository = new Repositories.TypeRepository(options);
            var _productRepository = new Repositories.ProductRepository(options);

            var SeedBasePath = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData");

            //Seed Brands
            if (await _brandRepository.CountAsync() == 0)
            {
                var data = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "brands.json"));
                var list = JsonSerializer.Deserialize<List<ProductBrand>>(data) ?? new List<ProductBrand>();
                await _brandRepository.CreateManyAsync(list);
            }

            //Seed Types
            if (await _typeRepository.CountAsync() == 0)
            {
                var data = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "types.json"));
                var list = JsonSerializer.Deserialize<List<ProductType>>(data) ?? new List<ProductType>();
                await _typeRepository.CreateManyAsync(list);
            }

            //Seed Products
            if (await _productRepository.CountAsync() == 0)
            {
                var data = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "products.json"));
                var list = JsonSerializer.Deserialize<List<Product>>(data) ?? new List<Product>();
                foreach (var product in list)
                {
                    //Reset Id to let Mongo generate one
                    product.Id = null;
                    //Default Created Date if not set
                    if (product.CreatedDate == default)
                        product.CreatedDate = DateTime.UtcNow;
                }
                await _productRepository.CreateManyAsync(list);
            }
        }
    }
}
