using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Platform.Core.Models;
using Platform.Core.Persistence.Entities;
using Platform.Core.Persistence.Repositories;
using Platform.Infrastructure.Persistence.Settings;
using System.Linq.Expressions;

namespace Platform.Infrastructure.Persistence.Repositories
{
    public abstract class MongoRepository<T> : IMongoRepository<T> where T : MongoEntity
    {
        protected readonly IMongoCollection<T> _collection;
        public MongoRepository(IOptions<DatabaseSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var db = client.GetDatabase(options.Value.DatabaseName);
            _collection = db.GetCollection<T>($"{typeof(T).Name}s"); // Use the plural form of the entity name as the collection name
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection
               .Find(_ => true)
               .ToListAsync();
        }
        public async Task<T> GetByIdAsync(string id)
        {
            return await _collection
                 .Find(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }
        public async Task<T> CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }
        public async Task<ICollection<T>> CreateManyAsync(ICollection<T> entities)
        {
            await _collection.InsertManyAsync(entities);
            return entities;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            var result = await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return result.IsAcknowledged && result.MatchedCount > 0;
        }
        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<Pagination<T>> ApplyDataFilters(FilterDefinition<T> filter, Dictionary<string, Expression<Func<T, object>>> sortMap, string sort
            , int pageIndex, int pageSize)
        {

            var totalItem = await _collection.CountDocumentsAsync(filter);

            // Determine the sort order based on the provided sort parameter
            var descending = sort.StartsWith('-');
            var key = descending ? sort[1..] : sort;
            var sortDefinition = sortMap.TryGetValue(key, out var expression)
                ? descending
                    ? Builders<T>.Sort.Descending(expression)
                    : Builders<T>.Sort.Ascending(expression)
                : Builders<T>.Sort.Ascending(x => x.Id);

            // Apply the filter, sort, and pagination to the query
            return new Pagination<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Count = (int)totalItem,
                TotalPages = (int)Math.Ceiling((double)totalItem / pageSize),
                Data = await _collection
                .Find(filter)
                .Sort(sortDefinition)
                .Skip((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync()
            };
        }

        public async Task<int> CountAsync()
        {
            return (int)await _collection.CountDocumentsAsync(_ => true);
        }
    }
}
