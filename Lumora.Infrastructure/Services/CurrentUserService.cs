using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Lumora.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string GetCaller()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.User.Identity?.IsAuthenticated != true)
            return "Program";

        var claims = httpContext.User.Claims.ToList();
        var email = GetClaimValue(claims, ClaimTypes.Email, "email");
        var name = GetClaimValue(claims, ClaimTypes.Name, "unique_name", "name");
        var userId = GetClaimValue(claims, ClaimTypes.NameIdentifier, "nameid", "sub");

        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(email))
            return $"{name} ({email})";

        return name ?? email ?? userId ?? "Program";
    }

    public UserTokenDetails? GetCurrentUserDetails()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.User.Identity?.IsAuthenticated != true)
            return null;

        return MapClaimsToUserTokenDetails(httpContext.User.Claims);
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

        var rawUserId = GetClaimValue(claimList, ClaimTypes.NameIdentifier, "nameid", "sub");
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
