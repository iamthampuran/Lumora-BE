using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Common.ValueObjects;
using Lumora.Domain.Enums;

namespace Lumora.Domain.Entities.Identity;

public class User : BaseEntity
{
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public DateTime? LastLoginAt { get; set; }
    public byte[] Salt { get; private set; } = null!;

    public bool IsTwoFactorEnabled { get; private set; }
    public string? TwoFactorSecret { get; private set; }    //encrypted
    public string? TwoFactorBackupCodes { get; private set; } //encrypted json

    //suspensionInfo
    public SuspensionInfo SuspensionInfo { get; private set; } = new();

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
