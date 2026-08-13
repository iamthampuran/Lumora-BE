using Lumora.Application.Configuration;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Domain.Entities.Identity;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Lumora.Domain.Enums;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;


namespace Lumora.Application.Services;

public class AuthService(IOptions<AppSettingsConfiguration> options, IRefreshTokenRepository refreshTokenRepository, IGenericRepository<User> userRepository,
    IMinioService minioService) : IAuthService
{
    public async Task<string> GenerateAccessTokenAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = Encoding.ASCII.GetBytes(options.Value.Security.Jwt.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, Enum.GetName(typeof(UserRole), user.Role)!)
        };

        if (user.Role == UserRole.Studio && user.StudioProfile != null)
        {
            claims.Add(new("studioId", user.StudioProfile.Id.ToString()));
            claims.Add(new(ClaimTypes.Name, user.StudioProfile.StudioName));
            //user.StudioProfile.LogoUrl ?? claims.Add(new("logoUrl", user.StudioProfile.LogoUrl)) ;
            if (user.StudioProfile.LogoUrl != null)
            {
                var presignedUrl = await minioService.GeneratePresignedUrlAsync(user.StudioProfile.LogoUrl);
                claims.Add(new Claim("logoUrl", presignedUrl));

            }
        }
        else if (user.Role == UserRole.Consumer && user.ConsumerProfile != null)
        {
            claims.Add(new("consumerId", user.ConsumerProfile.Id.ToString()));
            claims.Add(new(ClaimTypes.Name, user.ConsumerProfile.FullName));
            if (user.ConsumerProfile.PhotoUrl != null)
            {
                var presignedUrl = await minioService.GeneratePresignedUrlAsync(user.ConsumerProfile.PhotoUrl);
                claims.Add(new("avatarUrl", presignedUrl));
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(options.Value.Security.Jwt.AccessTokenExpiryMinutes),
            Issuer = "lumora",
            Audience = "lumora-api",
            SigningCredentials = new(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public (string refreshToken, RefreshToken entity) GenerateRefreshTokenAsync(User user)
    {
        var randomBytes = GenerateSalt(32);
        var token = Convert.ToBase64String(randomBytes);

        var hashedToken = Convert.ToBase64String(

            KeyDerivation.Pbkdf2(
                password: token,
                salt: randomBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32
                ));

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = hashedToken,
            ExpiresAt = DateTime.UtcNow.AddHours(options.Value.Security.Jwt.RefreshTokenExpiryHours),
            CreatedAt = DateTime.UtcNow
        };

        refreshTokenRepository.Add(refreshToken);

        return (token, refreshToken);
    }

    public (string passwordHash, byte[] salt) HashPasswordAsync(string passwordPlainText)
    {
        var salt = GenerateSalt(16);
        var hashedPasword = KeyDerivation.Pbkdf2(passwordPlainText, salt, KeyDerivationPrf.HMACSHA256, options.Value.Security.Pbkdf2.IterationCount,
            options.Value.Security.Pbkdf2.HashLength);
        return (Convert.ToBase64String(hashedPasword), salt);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenRepository.GetFirstAsync(rt => rt.Token == token && rt.RevokedAt == null);
        if (refreshToken == null)
            return false;
        refreshToken.RevokedAt = DateTime.UtcNow;
        return true;
    }

    public async Task<User?> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        var refreshToken = await refreshTokenRepository.GetFirstAsync(rt => rt.Token == token && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow, cancellationToken);
        if (refreshToken == null) return null;
        return await userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
    }

    public bool VerifyPasswordAsync(string passwordPlainText, string hashedPassword, byte[] salt)
    {
        var storedHash = Convert.FromBase64String(hashedPassword);
        var submittedHash = KeyDerivation.Pbkdf2(passwordPlainText, salt, KeyDerivationPrf.HMACSHA256, options.Value.Security.Pbkdf2.IterationCount,
            options.Value.Security.Pbkdf2.HashLength);
        return CryptographicOperations.FixedTimeEquals(storedHash, submittedHash);
    }

    private static byte[] GenerateSalt(int n = 16)
    {
        return RandomNumberGenerator.GetBytes(n);
    }

}
