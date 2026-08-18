using Platform.Core.Persistence.Entities;

namespace Platform.Core.Persistence.Repositories;

public interface ITranRepository<T> where T : Entity
{
    // Command
    // =========================================================
    void Create(T entity);
    void CreateMany(IEnumerable<T> entities);

    void Update(T entity);
    void UpdateMany(IEnumerable<T> entities);

    void Delete(T entity);
    void DeleteMany(IEnumerable<T> entities);
    void DeleteById(Guid id);
}