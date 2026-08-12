using Platform.Core.Models;
using Platform.Core.Persistence.Entities;
using Platform.Core.Persistence.Repositories;
using System.Linq.Expressions;

namespace Platform.Infrastructure.Persistence.Repositories;

public abstract class Repository<T> : IRepository<T>
    where T : Entity
{
    protected readonly PlatformDbContext _context;
    protected readonly DbSet<T> _dbSet;

    protected Repository(PlatformDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .FindAsync(id);
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);

        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<ICollection<T>> CreateManyAsync(
        ICollection<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);

        await _context.SaveChangesAsync();

        return entities;
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);

        var affectedRows =
            await _context.SaveChangesAsync();

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);

        if (entity is null)
            return false;

        _dbSet.Remove(entity);

        var affectedRows =
            await _context.SaveChangesAsync();

        return affectedRows > 0;
    }

    public async Task<Pagination<T>> ApplyDataFilters(
        IQueryable<T> query,
        Dictionary<string, Expression<Func<T, object>>> sortMap,
        string sort,
        int pageIndex,
        int pageSize)
    {
        var totalItems = await query.CountAsync();

        var descending = sort.StartsWith('-');

        var key = descending
            ? sort[1..]
            : sort;

        if (sortMap.TryGetValue(key, out var expression))
        {
            query = descending
                ? query.OrderByDescending(expression)
                : query.OrderBy(expression);
        }
        else
        {
            query = query.OrderBy(x => x.Id);
        }

        var data = await query
            .AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Pagination<T>
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Count = totalItems,
            TotalPages = (int)Math.Ceiling(
                (double)totalItems / pageSize),
            Data = data
        };
    }

    public async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }
}