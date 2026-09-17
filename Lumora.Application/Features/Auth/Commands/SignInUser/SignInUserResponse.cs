namespace Lumora.Application.Features.Auth.Commands.SignInUser;

public record SignInUserResponse(string? AccessToken, string RefreshToken, bool Requires2FA = false);
