using Lumora.Domain.Entities.Identity;
using System.Linq.Expressions;

namespace Lumora.Application.Contracts.Persistence;

public interface IRefreshTokenRepository
{
    Task<List<RefreshToken>> GetAsync(
        Expression<Func<RefreshToken, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetFirstAsync(
        Expression<Func<RefreshToken, bool>> predicate,
        CancellationToken cancellationToken = default);

    void Add(RefreshToken refreshToken);

    Task<int> RemoveRangeAsync(Expression<Func<RefreshToken, bool>> predicate, CancellationToken cancellationToken = default);
}
