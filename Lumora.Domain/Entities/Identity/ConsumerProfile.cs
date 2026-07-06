using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Entities.Reviews;

namespace Lumora.Domain.Entities.Identity;

public class ConsumerProfile : BaseEntity
{
    public Guid UserId { get; private set; }
    public string FullName { get; set; } = null!;
    public string? Phone {  get; set; }
    public string? PhotoUrl { get; set; }
    public string? Bio {  get; set; }

    //Navigation
    public virtual User User { get; private set; } = null!;
    public virtual ICollection<Event.Event> Events { get; set; } = [];
    public virtual ICollection<Inquiry> Inquiry { get; set; } = [];
    public virtual ICollection<Review> Reviews { get; set; } = [];
    
    public void UpdateProfile(string fullName, string? phone, string? bio)
    {
        FullName = fullName;
        Phone = phone;
        Bio = bio;
    }
}
