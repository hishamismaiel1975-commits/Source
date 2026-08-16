using Microsoft.EntityFrameworkCore;
using Platform.Core.Persistence.Entities;
using Platform.Core.Persistence.Repositories;

namespace Platform.Infrastructure.Persistence.EFCore;

public class EFTranRepository<T> : ITranRepository<T> where T : Entity
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public EFTranRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // Command
    // =========================================================
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
    public void DeleteById(Guid id)
    {
        var entity = _dbSet.FirstOrDefault(x => x.Id == id);
        if (entity is not null)
        {
            _dbSet.Remove(entity);
        }
    }


}