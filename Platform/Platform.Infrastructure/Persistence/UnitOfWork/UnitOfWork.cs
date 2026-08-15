using Microsoft.EntityFrameworkCore;
using Platform.Core.Persistence.Entities;
using Platform.Core.Persistence.Repositories;
using Platform.Core.UnitOfWork;
using Platform.Infrastructure.Persistence.Repositories;

namespace Platform.Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public IRepository<T> Repository<T>() where T : Entity
        {
            return new Repository<T>(_context);
        }
        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
