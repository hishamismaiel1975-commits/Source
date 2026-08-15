namespace Platform.Infrastructure.Persistence.MongoDB.Settings;

public class MongoDbSettings
{
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
}