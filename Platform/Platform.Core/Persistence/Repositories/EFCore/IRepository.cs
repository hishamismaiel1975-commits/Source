using Platform.Core.Models;
using Platform.Core.Persistence.Entities;
using System.Linq.Expressions;

namespace Platform.Core.Persistence.Repositories.EFCore;

public interface IRepository<T> where T : Entity
{
    // Query
    // =========================================================
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filters = null);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filters);
    Task<T?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> CountAsync();

    // Paging / Filtering / Sorting / Includes
    // sortBy desc when starts with '-' and asc when not
    // sortMap is a dictionary that maps the sortBy string to the corresponding expression
    // =========================================================
    Task<Pagination<T>> GetPagedAsync(IReadOnlyCollection<Expression<Func<T, bool>>>? filters, IReadOnlyCollection<Expression<Func<T, object>>>? includes,
     string? sortBy, IReadOnlyDictionary<string, Expression<Func<T, object>>> sortMap,
     int? pageIndex, int? pageSize);

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