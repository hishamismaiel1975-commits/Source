using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Platform.Core.Models;
using Platform.Core.Persistence.Entities;
using Platform.Core.Persistence.Repositories;
using Platform.Infrastructure.Persistence.MongoDB.Extensions;
using System.Linq.Expressions;

namespace Platform.Infrastructure.Persistence.MongoDB.Repositories;

public class MongoRepository<T> : IRepository<T> where T : Entity
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoClient client, IConfiguration configuration)
    {
        var database = client.GetDatabase(configuration["MongoDbSettings:DatabaseName"]);
        _collection = database.GetCollection<T>($"{typeof(T).Name}s");
    }

    // Query
    // =========================================================
    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, IReadOnlyCollection<Expression<Func<T, object>>>? includes = null)
    {
        var filterDef = filter is null
            ? Builders<T>.Filter.Empty
            : FilterBuilder.Build(filter);

        var query = _collection
            .Aggregate(new AggregateOptions
            {
                Collation = new Collation(
                    locale: "en",
                    strength: CollationStrength.Secondary)
            })
            .Match(filterDef);

        // Includes
        if (includes is not null)
        {
            foreach (var include in includes)
            {
                query = ApplyInclude(query, include);
            }
        }

        return await query.ToListAsync();
    }
    public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>>? filter = null)
    {
        var filterDef = filter is null
            ? Builders<T>.Filter.Empty
            : FilterBuilder.Build(filter);

        return await _collection
            .Aggregate(new AggregateOptions
            {
                Collation = new Collation(
                    locale: "en",
                    strength: CollationStrength.Secondary)
            })
            .Match(filterDef)
            .Project(select)
            .ToListAsync();
    }
    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, IReadOnlyCollection<Expression<Func<T, object>>>? includes = null)
    {
        var filterDef = FilterBuilder.Build(filter);
        var query = _collection
            .Aggregate(new AggregateOptions
            {
                Collation = new Collation(
                    locale: "en",
                    strength: CollationStrength.Secondary)
            })
            .Match(filterDef);

        if (includes is not null)
        {
            foreach (var include in includes)
            {
                query = ApplyInclude(query, include);
            }
        }

        return await query.FirstOrDefaultAsync();
    }
    public async Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> filter)
    {
        var filterDef = FilterBuilder.Build(filter);
        return await _collection
            .Aggregate(new AggregateOptions
            {
                Collation = new Collation(
                    locale: "en",
                    strength: CollationStrength.Secondary)
            })
            .Match(filterDef)
            .Project(select)
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

    // Paging / Filtering / Sorting / Includes
    // sortBy desc when starts with '-' and asc when not
    // sortMap is a dictionary that maps the sortBy string to the corresponding expression
    // =========================================================
    public async Task<Pagination<T>> GetPagedAsync(
        IReadOnlyCollection<Expression<Func<T, bool>>>? filters = null,
        IReadOnlyCollection<Expression<Func<T, object>>>? includes = null,
        string? sortBy = null,
        IReadOnlyDictionary<string, Expression<Func<T, object>>>? sortMap = null,
        int? pageIndex = null, int? pageSize = null)
    {
        var currentPage = pageIndex ?? 1;
        var currentPageSize = pageSize ?? 10;

        var builder = Builders<T>.Filter;
        var filterDef = builder.Empty;

        if (filters is not null && filters.Count > 0)
        {
            foreach (var expression in filters)
            {
                var mongoFilter =
                    FilterBuilder.Build(expression);

                filterDef &= mongoFilter;
            }
        }

        var totalCount = await _collection.CountDocumentsAsync(filterDef);
        var totalPages = (int)Math.Ceiling(
            (double)totalCount / currentPageSize);

        var query = _collection
            .Aggregate(new AggregateOptions
            {
                Collation = new Collation(locale: "en", strength: CollationStrength.Secondary)
            })
            .Match(filterDef);

        // Includes
        if (includes is not null)
        {
            foreach (var include in includes)
            {
                query = ApplyInclude(query, include);
            }
        }

        // Sorting
        if (!string.IsNullOrWhiteSpace(sortBy) && sortMap is not null)
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
    public async Task<Pagination<TResult>> GetPagedAsync<TResult>(
    Expression<Func<T, TResult>> select,
    IReadOnlyCollection<Expression<Func<T, bool>>>? filters = null,
    string? sortBy = null,
    IReadOnlyDictionary<string, Expression<Func<T, object>>>? sortMap = null,
    int? pageIndex = null,
    int? pageSize = null)
    where TResult : class
    {
        var currentPage = pageIndex ?? 1;
        var currentPageSize = pageSize ?? 10;

        var builder = Builders<T>.Filter;
        var filterDef = builder.Empty;

        if (filters is not null && filters.Count > 0)
        {
            foreach (var expression in filters)
            {
                var mongoFilter =
                    FilterBuilder.Build(expression);

                filterDef &= mongoFilter;
            }
        }

        var totalCount = await _collection.CountDocumentsAsync(filterDef);
        var totalPages = (int)Math.Ceiling(
            (double)totalCount / currentPageSize);

        var query = _collection
            .Aggregate(new AggregateOptions
            {
                Collation = new Collation(
                    locale: "en",
                    strength: CollationStrength.Secondary)
            })
            .Match(filterDef);

        // Sort
        if (!string.IsNullOrWhiteSpace(sortBy) &&
            sortMap is not null)
        {
            var descending = sortBy.StartsWith('-');
            var key = sortBy.TrimStart('-');

            if (sortMap.TryGetValue(key, out var sortExpression))
            {
                var sort = descending
                    ? Builders<T>.Sort.Descending(sortExpression)
                    : Builders<T>.Sort.Ascending(sortExpression);

                query = query.Sort(sort);
            }
        }

        // Projection
        var projectedQuery = query.Project(select);

        // Pagination
        var data = await projectedQuery
              .Skip((currentPage - 1) * currentPageSize)
              .Limit(currentPageSize)
              .ToListAsync();

        return new Pagination<TResult>(
            currentPage,
            currentPageSize,
            totalPages,
            (int)totalCount,
            data);
    }

    // Command
    // =========================================================
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

    private IAggregateFluent<T> ApplyInclude(
    IAggregateFluent<T> query,
    Expression<Func<T, object>> include)
    {
        var path = GetPropertyPath(include);
        var parts = path.Split('.');
        var currentPath = string.Empty;
        for (var i = 0; i < parts.Length; i++)
        {
            var property = parts[i];

            currentPath = string.IsNullOrEmpty(currentPath)
                ? property
                : $"{currentPath}.{property}";

            var parentPath = i == 0
                ? string.Empty
                : string.Join(".", parts.Take(i));

            var localField = string.IsNullOrEmpty(parentPath)
                ? $"{property}Id"
                : $"{parentPath}.{property}Id";

            var lookupStage = new BsonDocument(
                "$lookup",
                new BsonDocument
                {
                { "from", $"{property}s" },
                { "localField", localField },
                { "foreignField", "_id" },
                { "as", currentPath }
                });

            query = query.AppendStage<T>(lookupStage);

            var unwindStage = new BsonDocument(
                "$unwind",
                new BsonDocument
                {
                { "path", $"${currentPath}" },
                { "preserveNullAndEmptyArrays", true }
                });

            query = query.AppendStage<T>(unwindStage);
        }

        return query;
    }
    private static string GetPropertyPath<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var members = new Stack<string>();

        Expression? current = expression.Body;

        if (current is UnaryExpression unary)
            current = unary.Operand;

        while (current is MemberExpression member)
        {
            members.Push(member.Member.Name);
            current = member.Expression;
        }

        return string.Join(".", members);
    }

}