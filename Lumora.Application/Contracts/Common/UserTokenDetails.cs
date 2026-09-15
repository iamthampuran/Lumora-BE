namespace Lumora.Application.Contracts.Common;

public record UserTokenDetails(
    Guid UserId,
    string Email,
    string Role,
    string? Name,
    Guid? StudioId,
    Guid? ConsumerId,
    string? AvatarOrLogoUrl,
    bool? IsProfileComplete
);