using Microsoft.Extensions.DependencyInjection;
using Platform.Lib.Persistence.IRepositories;
using Reporting.Core.Persistence.Entities;
using System.Text.Json;

namespace Reporting.Infrastructure.Persistence.Seed
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var reportRepository = services.GetRequiredService<IRepository<Report>>();

            var seedBasePath = Path.Combine(AppContext.BaseDirectory, "Persistence", "Seed", "Data");

            var filePath = Path.Combine(seedBasePath, "Reports.json");

            var data = await File.ReadAllTextAsync(filePath);

            var reports = JsonSerializer.Deserialize<List<Report>>(data) ?? new List<Report>();

            var existingReports = await reportRepository.GetAllAsync();

            var existingIds = existingReports
                .Select(x => x.Id)
                .ToHashSet();

            var missingReports = reports
                .Where(x => !existingIds.Contains(x.Id))
                .ToList();

            if (missingReports.Count > 0)
            {
                await reportRepository.CreateManyAsync(missingReports);
            }
        }
    }
}
