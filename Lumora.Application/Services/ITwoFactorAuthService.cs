namespace Lumora.Application.Services;

public interface ITwoFactorAuthService
{
    (string secret, string qrCodeUri) GenerateSetupInfo(string email);
    bool ValidateCode(string secret, string code);
    List<string> GenerateBackupCodes();
}
