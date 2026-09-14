using Lumora.Application.Configuration;
using Lumora.Application.Contracts.Common;
using Lumora.Domain.Entities.Common;
using Lumora.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lumora.Infrastructure;

public class UnitOfWork(
    AppDbContext appDbContext,
    IHttpContextAccessor httpContextAccessor,
    IOptions<AppSettingsConfiguration> options) : IUnitOfWork
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

    public UserTokenDetails? GetCurrentUserDetails()
    {
        var claims = GetRequestClaims();
        return claims is null ? null : MapClaimsToUserTokenDetails(claims);
    }

    private string GetCaller()
    {
        var claims = GetRequestClaims();
        if (claims is null)
            return "Program";

        var email = GetClaimValue(claims, ClaimTypes.Email, "email");
        var name = GetClaimValue(claims, ClaimTypes.Name, "unique_name", "name");
        var userId = GetClaimValue(claims, ClaimTypes.NameIdentifier, "nameid", "sub");

        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(email))
            return $"{name} <{email}>";

        return name ?? email ?? userId ?? "Program";
    }

    private IEnumerable<Claim>? GetRequestClaims()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
            return null;

        if (httpContext.User.Identity?.IsAuthenticated == true)
            return httpContext.User.Claims;

        var bearerToken = ExtractBearerToken(httpContext);
        if (string.IsNullOrWhiteSpace(bearerToken))
            return null;

        return ValidateTokenAndGetClaims(bearerToken);
    }

    private IEnumerable<Claim>? ValidateTokenAndGetClaims(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var secret = Encoding.ASCII.GetBytes(options.Value.Security.Jwt.SecretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secret),
                ValidateIssuer = true,
                ValidIssuer = "lumora",
                ValidateAudience = true,
                ValidAudience = "lumora-api",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal.Claims;
        }
        catch
        {
            return null;
        }
    }

    private static string? ExtractBearerToken(HttpContext httpContext)
    {
        var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authorizationHeader))
            return null;

        const string bearerPrefix = "Bearer ";
        if (!authorizationHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            return null;

        return authorizationHeader[bearerPrefix.Length..].Trim();
    }

    private static string? GetClaimValue(IEnumerable<Claim> claims, params string[] claimTypes)
    {
        foreach (var claimType in claimTypes)
        {
            var value = claims.FirstOrDefault(c => c.Type == claimType)?.Value;
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return null;
    }

    private static UserTokenDetails? MapClaimsToUserTokenDetails(IEnumerable<Claim> claims)
    {
        var claimList = claims.ToList();

        var rawUserId = GetClaimValue(
            claimList,
            ClaimTypes.NameIdentifier,
            "nameid",
            "sub");

        if (!Guid.TryParse(rawUserId, out var userId))
            return null;

        var email = GetClaimValue(claimList, ClaimTypes.Email, "email") ?? string.Empty;
        var role = GetClaimValue(claimList, ClaimTypes.Role, "role") ?? string.Empty;
        var name = GetClaimValue(claimList, ClaimTypes.Name, "unique_name", "name");

        var studioIdClaim = GetClaimValue(claimList, "studioId");
        Guid? studioId = Guid.TryParse(studioIdClaim, out var parsedStudioId) ? parsedStudioId : null;

        var consumerIdClaim = GetClaimValue(claimList, "consumerId");
        Guid? consumerId = Guid.TryParse(consumerIdClaim, out var parsedConsumerId) ? parsedConsumerId : null;

        var avatarOrLogoUrl = GetClaimValue(claimList, "logoUrl", "avatarUrl");

        var isProfileCompleteClaim = GetClaimValue(claimList, "isProfileComplete");
        bool? isProfileComplete = bool.TryParse(isProfileCompleteClaim, out var parsedComplete) ? parsedComplete : null;

        return new UserTokenDetails(
            userId,
            email,
            role,
            name,
            studioId,
            consumerId,
            avatarOrLogoUrl,
            isProfileComplete);
    }
}
