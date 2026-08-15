using Platform.Core.Persistence.Entities;
using Platform.Core.Persistence.Repositories;

namespace Platform.Core.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : Entity;
        Task<int> SaveChangesAsync();
    }
}
