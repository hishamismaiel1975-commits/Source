using MongoDB.Driver;
using Platform.Core.Models;
using Platform.Core.Persistence.MongoDB.Entities;
using System.Linq.Expressions;

namespace Platform.Core.Persistence.MongoDB.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task<T> CreateAsync(T entity);
        Task<ICollection<T>> CreateManyAsync(ICollection<T> entities);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(string id);
        Task<Pagination<T>> ApplyDataFilters(FilterDefinition<T> filter, Dictionary<string, Expression<Func<T, object>>> sortMap, string sort, int pageIndex, int pageSize);

    }
}
