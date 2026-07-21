using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Common.ValueObjects;
using Lumora.Domain.Enums;

namespace Lumora.Domain.Entities.Identity;

public class User : BaseEntity
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public byte[] Salt { get; set; } = null!;

    public bool IsTwoFactorEnabled { get; set; }
    public string? TwoFactorSecret { get; set; }    //encrypted
    public string? TwoFactorBackupCodes { get; set; } //encrypted json

    //suspensionInfo
    public SuspensionInfo SuspensionInfo { get; set; } = new();

    //navigation properties

    public virtual ConsumerProfile? ConsumerProfile { get; private set; }
    public virtual StudioProfile? StudioProfile { get; private set; }
    public virtual List<RefreshToken> RefreshTokens { get; private set; } = [];

    public void SetPasswordHash(string hash)
        => this.PasswordHash = hash;

    public void SetLastLogin() => LastLoginAt = DateTime.UtcNow;

    public void Enable2FA(string secret)
    {
        TwoFactorSecret = secret;
        IsTwoFactorEnabled = true;
    }

    public void Suspend(string reason, DateTime? until, Guid suspendedBy)
    {
        SuspensionInfo = SuspensionInfo.CreateSuspension(reason, until, suspendedBy);
    }

    public void UnsuspendIfExpired()
    {
        if (SuspensionInfo.IsExpired())
            SuspensionInfo.LiftSuspension();
    }

}
