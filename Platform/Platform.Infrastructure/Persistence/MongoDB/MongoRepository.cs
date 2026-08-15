using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Platform.Core.Models;
using Platform.Core.Persistence.Entities;
using Platform.Core.Persistence.Repositories;
using System.Linq.Expressions;

namespace Platform.Infrastructure.Persistence.MongoDB;

public class MongoRepository<T> : IRepository<T>
    where T : Entity
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoClient client, IOptions<MongoDbSettings> options)
    {
        var database = client.GetDatabase(options.Value.DatabaseName);
        _collection = database.GetCollection<T>($"{typeof(T).Name}s");
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
    public async Task<Pagination<T>> GetPagedAsync(IEnumerable<Expression<Func<T, bool>>>? filters, IEnumerable<IncludeDefinition>? includes,
    string? sortBy, IReadOnlyDictionary<string, Expression<Func<T, object>>> sortMap,
    int? pageIndex, int? pageSize)
    {
        var currentPage = pageIndex ?? 1;
        var currentPageSize = pageSize ?? 10;

        var filterDef = Builders<T>.Filter.Empty;

        if (filters != null)
        {
            foreach (var expression in filters)
            {
                filterDef &= Builders<T>.Filter.Where(expression);
            }
        }

        var totalCount = await _collection.CountDocumentsAsync(filterDef);

        var totalPages = (int)Math.Ceiling(
            (double)totalCount / currentPageSize);

        var query = _collection
              .Aggregate()
              .Match(filterDef);

        // Includes //need to test it
        if (includes != null)
        {
            foreach (var include in includes)
            {
                var lookupStage = new BsonDocument("$lookup",
                    new BsonDocument
                    {
                { "from", $"{include.ForeignEntity}s" },
                { "localField", include.PrimaryField },
                { "foreignField", "_id" },
                { "as", $"{include.ForeignEntity}s" }
                    });

                query = query.AppendStage<T>(lookupStage);
            }
        }

        // Sorting
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
            .Skip((currentPage - 1) * currentPageSize)
            .Limit(currentPageSize)
            .ToListAsync();

        return new Pagination<T>(
            currentPage,
            currentPageSize,
            totalPages,
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

    public async Task test(string foreignEntity, string primaryField)
    {

        var result = await _collection.Aggregate()
                .Match(x => x.Id == new Guid("6f4c5131-4565-4e29-bb8e-c75c97e9b037"))
                .Lookup($"{foreignEntity}s", primaryField, "_id", foreignEntity)
                .FirstOrDefaultAsync();

    }

    public class ProductBrand : Entity
    {
        public required string Name { get; set; }
    }


}