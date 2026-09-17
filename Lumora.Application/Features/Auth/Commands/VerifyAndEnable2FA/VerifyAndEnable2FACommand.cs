namespace Lumora.Application.Features.Auth.Commands.VerifyAndEnable2FA;

public record VerifyAndEnable2FACommand(string Secret, string Code);