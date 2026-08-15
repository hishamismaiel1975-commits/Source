using Platform.Core.Models;
using Platform.Core.Persistence.Entities;
using System.Linq.Expressions;

namespace Platform.Core.Persistence.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        // Query
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> CountAsync();

        // Command
        void Create(T entity);
        void CreateMany(IEnumerable<T> entities);

        void Update(T entity);
        void UpdateMany(IEnumerable<T> entities);

        void Delete(T entity);
        void DeleteMany(IEnumerable<T> entities);
        Task DeleteByIdAsync(Guid id);

        // Sort & Filtering & Pagination
        Task<Pagination<T>> ApplyDataFiltersAsync(IQueryable<T> filter, Dictionary<string, Expression<Func<T, object>>> sortMap, string sort, int pageIndex, int pageSize);

    }
}
