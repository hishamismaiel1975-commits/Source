using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Catalog.Infrastructure.Persistence.SQLServer;

// For Developing Migrations Only
public class CatalogDbContextFactory
    : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Server=localhost,2002;Database=CatalogDb;User Id=sa;Password=pass_123456789;TrustServerCertificate=True";

        var optionsBuilder =
            new DbContextOptionsBuilder<CatalogDbContext>();

        optionsBuilder.UseSqlServer(connectionString);

        return new CatalogDbContext(optionsBuilder.Options);
    }
}