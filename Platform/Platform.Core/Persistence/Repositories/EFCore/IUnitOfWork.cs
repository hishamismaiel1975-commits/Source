namespace Platform.Core.Persistence.Repositories.EFCore
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
