using Microsoft.EntityFrameworkCore;
using Platform.Lib.Persistence.IRepositories;

namespace Platform.Lib.Infrastructure.Persistence.EFCore.Repositories
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private readonly TContext _context;

        public UnitOfWork(TContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
