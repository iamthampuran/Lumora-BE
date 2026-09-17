using Lumora.Application.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using OtpNet;
using QRCoder;
using System.Security.Cryptography;
using System.Text.Json;

namespace Lumora.Infrastructure.Services;

public class TwoFactorAuthService : ITwoFactorAuthService
{
    private const string BackupCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int BackupCodeLength = 8;
    private const int BackupCodeHashIterations = 100_000;
    private const int BackupCodeHashBytes = 32;
    private const int BackupCodeSaltBytes = 16;

    private sealed record StoredBackupCode(string Salt, string Hash);

    public List<string> GenerateBackupCodes()
    {
        var codes = new List<string>(10);
        for (int i = 0; i < 10; i++)
        {
            var raw = GenerateRandomCode(BackupCodeLength);
            codes.Add($"{raw[..4]}-{raw[4..]}");
        }
        return codes;
    }

    public string HashBackupCodes(IEnumerable<string> plainBackupCodes)
    {
        var stored = new List<StoredBackupCode>();

        foreach (var code in plainBackupCodes)
        {
            var normalized = NormalizeBackupCode(code);
            var salt = RandomNumberGenerator.GetBytes(BackupCodeSaltBytes);
            var hash = KeyDerivation.Pbkdf2(
                password: normalized,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: BackupCodeHashIterations,
                numBytesRequested: BackupCodeHashBytes);

            stored.Add(new StoredBackupCode(
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash)));
        }

        return JsonSerializer.Serialize(stored);
    }

    public bool TryConsumeBackupCode(string submittedCode, string? storedBackupCodesJson, out string updatedBackupCodesJson)
    {
        updatedBackupCodesJson = storedBackupCodesJson ?? "[]";

        if (string.IsNullOrWhiteSpace(storedBackupCodesJson))
            return false;

        var submitted = NormalizeBackupCode(submittedCode);
        var stored = JsonSerializer.Deserialize<List<StoredBackupCode>>(storedBackupCodesJson) ?? [];

        for (int i = 0; i < stored.Count; i++)
        {
            var entry = stored[i];
            var salt = Convert.FromBase64String(entry.Salt);

            var candidateHash = KeyDerivation.Pbkdf2(
                password: submitted,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: BackupCodeHashIterations,
                numBytesRequested: BackupCodeHashBytes);

            var storedHash = Convert.FromBase64String(entry.Hash);

            if (CryptographicOperations.FixedTimeEquals(candidateHash, storedHash))
            {
                stored.RemoveAt(i); // one-time use
                updatedBackupCodesJson = JsonSerializer.Serialize(stored);
                return true;
            }
        }

        return false;
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

    private static string NormalizeBackupCode(string code) =>
        code.Replace("-", "", StringComparison.Ordinal)
            .Trim()
            .ToUpperInvariant();

    private static string GenerateRandomCode(int length)
    {
        Span<byte> bytes = stackalloc byte[length];
        RandomNumberGenerator.Fill(bytes);

        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = BackupCodeAlphabet[bytes[i] % BackupCodeAlphabet.Length];
        }

        return new string(chars);
    }
}