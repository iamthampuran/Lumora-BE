using Lumora.Application.Contracts.Persistence;
using Lumora.Domain.Entities.Identity;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lumora.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RefreshToken>> GetAsync(
        Expression<Func<RefreshToken, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetFirstAsync(
        Expression<Func<RefreshToken, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public void Add(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
    }
}