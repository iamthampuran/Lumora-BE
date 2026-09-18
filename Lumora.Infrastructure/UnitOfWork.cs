using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Services;
using Lumora.Domain.Entities.Common;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure;

public class UnitOfWork(
    AppDbContext appDbContext,
    ICurrentUserService currentUserService) : IUnitOfWork
{
    public async Task ExecuteTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        var strategy = appDbContext.Database.CreateExecutionStrategy();
        await strategy.Execute(async () =>
        {
            await using var transaction = await appDbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await action();
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var caller = currentUserService.GetCaller();

        foreach (var entry in appDbContext.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case Microsoft.EntityFrameworkCore.EntityState.Modified:
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = caller;
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = caller;
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = caller;
                    break;
            }
        }

        return await appDbContext.SaveChangesAsync(cancellationToken);
    }
}