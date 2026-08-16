using Catalog.Core.Persistence.MongoDB.Entities;
using Microsoft.Extensions.DependencyInjection;
using Platform.Core.Persistence.Repositories;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var _brandRepository = services.GetRequiredService<IRepository<Brand>>();
            var _typeRepository = services.GetRequiredService<IRepository<Core.Persistence.MongoDB.Entities.Type>>();
            var _productRepository = services.GetRequiredService<IRepository<Product>>();

            var SeedBasePath = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData");

            //Seed Brands
            if (await _brandRepository.CountAsync() == 0)
            {
                var data = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "brands.json"));
                var list = JsonSerializer.Deserialize<List<Brand>>(data) ?? new List<Brand>();
                await _brandRepository.CreateManyAsync(list);
            }

            //Seed Types
            if (await _typeRepository.CountAsync() == 0)
            {
                var data = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "types.json"));
                var list = JsonSerializer.Deserialize<List<Core.Persistence.MongoDB.Entities.Type>>(data) ?? new List<Core.Persistence.MongoDB.Entities.Type>();
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
                    product.CreatedDate = DateTime.UtcNow;
                }
                await _productRepository.CreateManyAsync(list);
            }
        }
    }
}
