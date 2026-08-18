using Microsoft.EntityFrameworkCore;
using Platform.Core.Models;
using Platform.Core.Persistence.Entities;
using Platform.Core.Persistence.Repositories;
using System.Linq.Expressions;

namespace Platform.Infrastructure.Persistence.EFCore;

public class EFRepository<T> : IRepository<T> where T : Entity
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;
    public EFRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // Query
    // =========================================================
    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
    {
        IQueryable<T> query = _dbSet;

        if (predicate is not null)
            query = query.Where(predicate);

        return await query.ToListAsync();
    }
    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet
            .FirstOrDefaultAsync(predicate);
    }
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet
            .AnyAsync(x => x.Id == id);
    }
    public async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    // Paging / Filtering / Sorting / Includes
    // sortBy desc when starts with '-' and asc when not
    // sortMap is a dictionary that maps the sortBy string to the corresponding expression
    // =========================================================
    public async Task<Pagination<T>> GetPagedAsync(IReadOnlyCollection<Expression<Func<T, bool>>>? filters, IReadOnlyCollection<Expression<Func<T, object>>>? includes,
        string? sortBy, IReadOnlyDictionary<string, Expression<Func<T, object>>> sortMap, int? pageIndex, int? pageSize)
    {
        var currentPage = pageIndex ?? 1;
        var currentPageSize = pageSize ?? 10;

        IQueryable<T> query = _dbSet;


        // Filters
        // -----------------------------------------------------
        if (filters != null)
        {
            foreach (var filter in filters)
            {
                query = query.Where(filter);
            }
        }

        // Includes
        // -----------------------------------------------------
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        // Total Count
        // -----------------------------------------------------
        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(
            (double)totalCount / currentPageSize);

        // Sorting
        // -----------------------------------------------------
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            var descending = sortBy.StartsWith('-');

            var key = descending
                ? sortBy[1..]
                : sortBy;

            if (sortMap.TryGetValue(key, out var sortExpression))
            {
                query = descending
                    ? query.OrderByDescending(sortExpression)
                    : query.OrderBy(sortExpression);
            }
        }

        // Paging
        // -----------------------------------------------------
        var data = await query
            .Skip((currentPage - 1) * currentPageSize)
            .Take(currentPageSize)
            .ToListAsync();

        return new Pagination<T>(
            currentPage,
            currentPageSize,
            totalPages,
            totalCount,
            data);
    }

    // Command
    // =========================================================
    public async Task CreateAsync(T entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
    }
    public async Task CreateManyAsync(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateManyAsync(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteManyAsync(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteByIdAsync(Guid id)
    {
        var entity = await _dbSet
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity is not null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

    }


}