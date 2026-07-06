namespace Lumora.Domain.Entities.Common.ValueObjects;

public class SuspensionInfo
{
    public bool IsSuspended { get; set; }
    public DateTime? SuspendedAt { get; set; }
    public string? SuspensionReason { get; set; }
    public DateTime? SuspendedTill { get; set; }
    public Guid? SuspendedBy { get; set; } //FK to admin User

    public static SuspensionInfo CreateSuspension(string reason, DateTime? until, Guid suspendedBy)
    {
        return new()
        {
            IsSuspended = true,
            SuspendedAt = DateTime.UtcNow,
            SuspensionReason = reason,
            SuspendedTill = until,
            SuspendedBy = suspendedBy,
        };
    }

    public void LiftSuspension()
    {
        IsSuspended = false;
        SuspendedTill = null;
    }

    public bool IsExpired() =>
        SuspendedTill.HasValue && SuspendedTill.Value <= DateTime.UtcNow && IsSuspended;
}
