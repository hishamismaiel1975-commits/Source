using Platform.Core.Models;
using Platform.Core.Persistence.Entities;
using System.Linq.Expressions;

namespace Platform.Core.Persistence.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<T> CreateAsync(T entity);
        Task<ICollection<T>> CreateManyAsync(ICollection<T> entities);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);
        Task<Pagination<T>> ApplyDataFilters(IQueryable<T> filter, Dictionary<string, Expression<Func<T, object>>> sortMap, string sort, int pageIndex, int pageSize);
    }
}
