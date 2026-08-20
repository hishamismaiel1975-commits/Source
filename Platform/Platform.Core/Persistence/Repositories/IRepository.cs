using Platform.Core.Models;
using Platform.Core.Persistence.Entities;
using System.Linq.Expressions;

namespace Platform.Core.Persistence.Repositories;

public interface IRepository<T> where T : Entity
{
    // Query
    // =========================================================
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, IReadOnlyCollection<Expression<Func<T, object>>>? includes = null);
    Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>>? filter = null);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, IReadOnlyCollection<Expression<Func<T, object>>>? includes = null);
    Task<TResult?> FirstOrDefaultAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, bool>> filter);
    Task<T?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> CountAsync();

    // Paging / Filtering / Sorting / Includes
    // sortBy desc when starts with '-' and asc when not
    // sortMap is a dictionary that maps the sortBy string to the corresponding expression
    // =========================================================
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