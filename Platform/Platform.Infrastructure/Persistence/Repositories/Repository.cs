using Microsoft.EntityFrameworkCore;
using Platform.Core.Models;
using Platform.Core.Persistence.Entities;
using Platform.Core.Persistence.Repositories;
using System.Linq.Expressions;

namespace Platform.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T>
    where T : Entity
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // Query
    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
    {
        IQueryable<T> query = _dbSet;

        if (predicate != null)
            query = query.Where(predicate);

        return await query.ToListAsync();
    }
    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.FirstOrDefaultAsync(predicate);
    }
    public async Task<T?> GetByIdAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity;
    }
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(x => x.Id == id);
    }
    public async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    // Command
    public void Create(T entity)
    {
        _dbSet.Add(entity);
    }
    public void CreateMany(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
    public void UpdateMany(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
    public void DeleteMany(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
    public async Task DeleteByIdAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);

        if (entity is null)
            return;

        _dbSet.Remove(entity);
    }


    // Sort & Filtering & Pagination
    public async Task<Pagination<T>> ApplyDataFiltersAsync(IQueryable<T> query, Dictionary<string, Expression<Func<T, object>>> sortMap, string sort, int pageIndex, int pageSize)
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

}