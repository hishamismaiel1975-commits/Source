using Microsoft.Extensions.Caching.Distributed;
using Platform.Lib.Core.Persistence.Repositories;
using System.Text.Json;

public class RedisRepository<T> : ICacheRepository<T>
    where T : class
{
    private readonly IDistributedCache _cache;

    public RedisRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        var json = await _cache.GetStringAsync(BuildKey(id));

        return json is null
            ? null
            : JsonSerializer.Deserialize<T>(json);
    }
    public async Task<IReadOnlyList<T>?> GetAllAsync(string id)
    {
        var json = await _cache.GetStringAsync(BuildKey(id));

        return json is null
            ? null
            : JsonSerializer.Deserialize<List<T>>(json);
    }
    public async Task SetAsync(string id, T entity, TimeSpan? expiration = null)
    {
        var json = JsonSerializer.Serialize(entity);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)

        };

        await _cache.SetStringAsync(
            BuildKey(id),
            json,
            options);
    }
    public async Task SetAllAsync(string key, IEnumerable<T> entities, TimeSpan? expiration = null)
    {
        var json = JsonSerializer.Serialize(entities);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30),
        };

        await _cache.SetStringAsync(BuildKey(key), json, options);
    }
    public Task DeleteAsync(string id)
    {
        return _cache.RemoveAsync(BuildKey(id));
    }
    private static string BuildKey(string id)
    {
        return $"{typeof(T).Name}:{id}";
    }

}