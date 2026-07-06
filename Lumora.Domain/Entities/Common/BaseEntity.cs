namespace Lumora.Domain.Entities.Common;

public class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAt { get;  set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get;  set; } = DateTime.UtcNow;
    public bool IsActive { get; set; }
    public DateTime? DeletedAt { get; private set; }
    public string CreatedBy { get;  set; } = null!;
    public string ModifiedBy { get;  set; } = null!;

}
