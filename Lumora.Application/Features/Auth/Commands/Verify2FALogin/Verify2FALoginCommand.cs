namespace Lumora.Application.Features.Auth.Commands.Verify2FALogin;

public record Verify2FALoginCommand(string Email, string Code);