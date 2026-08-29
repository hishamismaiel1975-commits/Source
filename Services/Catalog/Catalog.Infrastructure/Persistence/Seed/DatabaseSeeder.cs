using Catalog.Core.Persistence.Entities;
using Microsoft.Extensions.DependencyInjection;
using Platform.Core.Persistence.Repositories;
using Platform.Core.Time;
using System.Text.Json;

namespace Catalog.Infrastructure.Persistence.Seed
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var _brandRepository = services.GetRequiredService<IRepository<ProductBrand>>();
            var _typeRepository = services.GetRequiredService<IRepository<ProductType>>();
            var _productRepository = services.GetRequiredService<IRepository<Product>>();

            var SeedBasePath = Path.Combine(AppContext.BaseDirectory, "Persistence", "Seed", "Data");

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
                    //Set Created Date
                    product.CreatedDate = SaudiTime.Now;
                }
                await _productRepository.CreateManyAsync(list);
            }
        }
    }
}
