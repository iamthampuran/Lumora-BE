using Lumora.Domain.Entities.Identity;

namespace Lumora.Application.Contracts.Services;

public interface IAuthService
{
    (string passwordHash, byte[] salt) HashPasswordAsync(string passwordPlainText);
    bool VerifyPasswordAsync(string passwordPlainText, string hashedPassword, byte[] salt);
    string GenerateAccessTokenAsync(User user);
    (string refreshToken, RefreshToken entity) GenerateRefreshTokenAsync(User user);
    Task<User?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken);
    Task<bool> RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken);
}
