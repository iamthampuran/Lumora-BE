using Lumora.Application.Contracts.Common;
using Lumora.Domain.Entities.Common;
using Lumora.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Lumora.Infrastructure;

public class UnitOfWork(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor) : IUnitOfWork
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
        var caller = GetCaller();
        foreach (var entry in appDbContext.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                //case Microsoft.EntityFrameworkCore.EntityState.Detached:
                //    break;
                //case Microsoft.EntityFrameworkCore.EntityState.Unchanged:
                //    break;
                //case Microsoft.EntityFrameworkCore.EntityState.Deleted:
                //    break;
                case Microsoft.EntityFrameworkCore.EntityState.Modified:
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = caller;
                    break;
                case Microsoft.EntityFrameworkCore.EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = caller;
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = caller;
                    entry.Entity.IsActive = true;
                    break;
            }
        }

        return await appDbContext.SaveChangesAsync(cancellationToken);
    }

    private string GetCaller()
    {
        var identity = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
        var userName = identity?.FindFirst("name")?.Value;
        var email = identity?.FindFirst("email")?.Value;

        return userName != null && email != null ? $"{userName} {email}" : string.Empty;
    }
}
