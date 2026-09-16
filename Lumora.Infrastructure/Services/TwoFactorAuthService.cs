using Lumora.Application.Services;
using OtpNet;
using QRCoder;

namespace Lumora.Infrastructure.Services;

public class TwoFactorAuthService : ITwoFactorAuthService
{
    public List<string> GenerateBackupCodes()
    {
        var codes = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            var code = Path.GetRandomFileName().Replace(".", "").Substring(0, 8).ToUpper();
            codes.Add($"{code.Substring(0, 4)}-{code.Substring(4, 4)}");
        }
        return codes;
    }

    public (string secret, string qrCodeUri) GenerateSetupInfo(string email)
    {
        var secretKey = KeyGeneration.GenerateRandomKey(20);
        var secretString = Base32Encoding.ToString(secretKey);

        var uriString = new OtpUri(OtpType.Totp, secretKey, email, "lumora").ToString();

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(uriString, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(20);
        var qrCodeUri = $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";

        return (secretString, qrCodeUri);

    }

    public bool ValidateCode(string secret, string code)
    {
        var secretBytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(secretBytes);
        return totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
    }
}
