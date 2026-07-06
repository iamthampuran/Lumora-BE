namespace Lumora.Application.Contracts.Common;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task ExecuteTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
}
