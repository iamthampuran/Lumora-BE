namespace Lumora.Application.Features.Auth.Queries.GetConsumerData;

public record GetConsumerDataResponse(string? ProfileUrl, string FullName, string Email, string? PhoneNumber, string? Bio, bool IsTwoFactorEnabled);
