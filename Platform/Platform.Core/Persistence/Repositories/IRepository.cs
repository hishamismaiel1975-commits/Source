using Platform.Core.Models;
using Platform.Core.Persistence.Entities;
using System.Linq.Expressions;

namespace Platform.Core.Persistence.Repositories;


public interface IRepository<T> where T : Entity
{
    // Query
    // =========================================================

    /// <summary>Dynamically applies Include and ThenInclude to nested navigation properties.</summary>
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, IReadOnlyCollection<Expression<Func<T, object>>>? includes = null);
    Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>>? filter = null);

    /// <summary>Dynamically applies Include and ThenInclude to nested navigation properties.</summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, IReadOnlyCollection<Expression<Func<T, object>>>? includes = null);
    Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> filter);
    Task<T?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> CountAsync();

    /// <summary>
    /// Paging / Filtering / Sorting / Includes.
    /// Supports descending sort when sortBy starts with '-' and ascending otherwise.
    /// sortMap maps the sortBy string to the corresponding expression.
    /// Dynamically applies Include and ThenInclude to nested navigation properties.
    /// </summary>
    /// =========================================================
    Task<Pagination<T>> GetPagedAsync(
        IReadOnlyCollection<Expression<Func<T, bool>>>? filters = null,
        IReadOnlyCollection<Expression<Func<T, object>>>? includes = null,
        string? sortBy = null,
        IReadOnlyDictionary<string, Expression<Func<T, object>>>? sortMap = null,
        int? pageIndex = null, int? pageSize = null);
    Task<Pagination<TResult>> GetPagedAsync<TResult>(
    Expression<Func<T, TResult>> select,
    IReadOnlyCollection<Expression<Func<T, bool>>>? filters = null,
    string? sortBy = null,
    IReadOnlyDictionary<string, Expression<Func<T, object>>>? sortMap = null,
    int? pageIndex = null,
    int? pageSize = null)
    where TResult : class;

    // Command
    // =========================================================
    Task CreateAsync(T entity);
    Task CreateManyAsync(IEnumerable<T> entities);

    Task UpdateAsync(T entity);
    Task UpdateManyAsync(IEnumerable<T> entities);

    Task DeleteAsync(T entity);
    Task DeleteManyAsync(IEnumerable<T> entities);
    Task DeleteByIdAsync(Guid id);
}