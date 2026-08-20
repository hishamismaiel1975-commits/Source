using Microsoft.EntityFrameworkCore;
using Platform.Core.Models;
using Platform.Core.Persistence.Entities;
using Platform.Core.Persistence.Repositories;
using Platform.Infrastructure.Persistence.EFCore.Extensions;
using System.Linq.Expressions;

namespace Platform.Infrastructure.Persistence.EFCore.Repositories;

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
    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, IReadOnlyCollection<Expression<Func<T, object>>>? includes = null)
    {
        IQueryable<T> query = _dbSet;

        // Includes
        // -----------------------------------------------------
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.IncludePath(include);
            }
        }

        if (filter is not null)
            query = query.Where(filter);

        return await query.ToListAsync();
    }
    public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _dbSet;

        if (filter is not null)
            query = query.Where(filter);

        return await query
            .Select(select)
            .ToListAsync();
    }
    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, IReadOnlyCollection<Expression<Func<T, object>>>? includes = null)
    {
        IQueryable<T> query = _dbSet;

        // Includes
        // -----------------------------------------------------
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.IncludePath(include);
            }
        }

        return await query
            .FirstOrDefaultAsync(filter);
    }
    public async Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> filter)
    {
        IQueryable<T> query = _dbSet;

        // Filter
        // -----------------------------------------------------
        query = query.Where(filter);

        // Projection
        // -----------------------------------------------------
        return await query
            .Select(select)
            .FirstOrDefaultAsync();
    }
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .FindAsync(id);
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
    public async Task<Pagination<T>> GetPagedAsync(
        IReadOnlyCollection<Expression<Func<T, bool>>>? filters = null,
        IReadOnlyCollection<Expression<Func<T, object>>>? includes = null,
        string? sortBy = null,
        IReadOnlyDictionary<string,
        Expression<Func<T, object>>>? sortMap = null,
        int? pageIndex = null, int? pageSize = null)
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
                query = query.IncludePath(include);
            }
        }

        // Total Count
        // -----------------------------------------------------
        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(
            (double)totalCount / currentPageSize);

        // Sorting
        // -----------------------------------------------------
        if (!string.IsNullOrWhiteSpace(sortBy) && sortMap != null)
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

        IQueryable<T> query = _dbSet;

        // Filters
        if (filters != null)
        {
            foreach (var filter in filters)
            {
                query = query.Where(filter);
            }
        }

        // Total Count
        var totalCount = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(
            (double)totalCount / currentPageSize);

        // Sorting
        if (!string.IsNullOrWhiteSpace(sortBy) && sortMap != null)
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

        // Paging + Projection
        var data = await query
            .Skip((currentPage - 1) * currentPageSize)
            .Take(currentPageSize)
            .Select(select)
            .ToListAsync();

        return new Pagination<TResult>(
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