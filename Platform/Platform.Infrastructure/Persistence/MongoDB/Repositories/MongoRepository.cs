using MongoDB.Driver;
using Platform.Core.Models;
using Platform.Core.Persistence.Entities;
using Platform.Core.Persistence.Repositories;
using System.Linq.Expressions;

namespace Platform.Infrastructure.Persistence.MongoDB.Repositories;

public class MongoRepository<T> : IRepository<T>
    where T : Entity
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<T>(typeof(T).Name);
    }

    // Query
    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
    {
        var filter = predicate is null
            ? Builders<T>.Filter.Empty
            : Builders<T>.Filter.Where(predicate);

        return await _collection
            .Find(filter)
            .ToListAsync();
    }
    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _collection
            .Find(predicate)
            .FirstOrDefaultAsync();
    }
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _collection
            .Find(x => x.Id == id)
            .Limit(1)
            .AnyAsync();
    }
    public async Task<int> CountAsync()
    {
        var count = await _collection.CountDocumentsAsync(
            Builders<T>.Filter.Empty);

        return (int)count;
    }

    //sortBy desc when starts with '-' and asc when not, sortMap is a dictionary that maps the sortBy string to the corresponding expression
    public async Task<Pagination<T>> GetPagedAsync(IEnumerable<Expression<Func<T, bool>>>? filters, string? sortBy, IReadOnlyDictionary<string, Expression<Func<T, object>>> sortMap,
                                                   int? pageIndex, int? pageSize)
    {
        var filterDef = Builders<T>.Filter.Empty;

        if (filters != null)
        {
            foreach (var expression in filters)
            {
                filterDef &= Builders<T>.Filter.Where(expression);
            }
        }

        var totalCount = await _collection.CountDocumentsAsync(filterDef);

        var query = _collection.Find(filterDef);

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            var descending = sortBy.StartsWith('-');
            var key = descending ? sortBy[1..] : sortBy;

            if (sortMap.TryGetValue(key, out var sortExpression))
            {
                query = descending
                    ? query.SortByDescending(sortExpression)
                    : query.SortBy(sortExpression);
            }
        }

        var data = await query
            .Skip((pageIndex - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return new Pagination<T>(
            pageIndex ?? 1,
            pageSize ?? 10,
            (int)totalCount,
            data);
    }

    // Command
    public async Task CreateAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }
    public async Task CreateManyAsync(IEnumerable<T> entities)
    {
        await _collection.InsertManyAsync(entities);
    }

    public async Task UpdateAsync(T entity)
    {
        await _collection.ReplaceOneAsync(
            x => x.Id == entity.Id,
            entity);
    }
    public async Task UpdateManyAsync(IEnumerable<T> entities)
    {
        var models = entities.Select(entity =>
            new ReplaceOneModel<T>(
                Builders<T>.Filter.Eq(x => x.Id, entity.Id),
                entity));

        await _collection.BulkWriteAsync(models);
    }

    public async Task DeleteAsync(T entity)
    {
        await _collection.DeleteOneAsync(x => x.Id == entity.Id);
    }
    public async Task DeleteManyAsync(IEnumerable<T> entities)
    {
        var ids = entities
            .Select(x => x.Id)
            .ToList();

        if (ids.Count == 0)
            return;

        var filter = Builders<T>.Filter.In(
            x => x.Id,
            ids);

        await _collection.DeleteManyAsync(filter);
    }
    public async Task DeleteByIdAsync(Guid id)
    {
        await _collection.DeleteOneAsync(
            x => x.Id == id);
    }


}