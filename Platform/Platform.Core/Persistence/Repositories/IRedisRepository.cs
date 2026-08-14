public interface IRedisRepository<T>
    where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<IReadOnlyList<T>?> GetAllAsync(string key);
    Task SetAsync(string id, T entity, TimeSpan? expiration = null);
    Task SetAllAsync(string key, IEnumerable<T> entities, TimeSpan? expiration = null);
    Task DeleteAsync(string id);
}