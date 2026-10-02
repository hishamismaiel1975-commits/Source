//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;

//namespace Catalog.Infrastructure.Persistence.PostgreSQL;

//// For Developing Migrations Only
//public class CatalogPostgresDbContextFactory
//    : IDesignTimeDbContextFactory<CatalogPostgresDbContext>
//{
//    public CatalogPostgresDbContext CreateDbContext(string[] args)
//    {
//        var connectionString = "Host=localhost;Port=2003;Database=CatalogDb;Username=admin;Password=pass_123456789";

//        var optionsBuilder =
//            new DbContextOptionsBuilder<CatalogPostgresDbContext>();

//        optionsBuilder.UseNpgsql(connectionString);

//        return new CatalogPostgresDbContext(optionsBuilder.Options);
//    }
//}