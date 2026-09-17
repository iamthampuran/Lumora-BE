namespace Lumora.Application.Features.Auth.Commands.Initiate2FASetup;

public record Initiate2FASetupResponse(string Secret, string QrCodeUri);
