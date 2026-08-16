using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Common.ValueObjects;
using Lumora.Domain.Entities.Identity;
using Lumora.Domain.Entities.Reviews;
using Lumora.Domain.Enums;

namespace Lumora.Domain.Entities.Event;

public class Event : BaseEntity
{
    public Guid ConsumerId { get; private set; }
    public Guid? SelectedStudioId { get; set; }
    public string Title { get; set; } = null!;
    public DateOnly EventDate {  get; set; }
    public Coordinates Location { get; private set; } = null!;
    public Guid EventTypeId {  get; set; }
    public decimal Budget { get; set; }
    public decimal Duration { get; private set; } //in hours
    public string? PhotographyStyle {  get; set; }
    public string? SpecialRequirements { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Created;

    //navigation properties
    public virtual EventType EventType { get; set; } = null!;

    public virtual ConsumerProfile Consumer { get; set; } = null!;
    public virtual StudioProfile? SelectedStudio { get; set; }
    public virtual ICollection<Inquiry> Inquiries { get; set; } = [];
    public virtual ICollection<Review> Reviews { get; set; } = [];
    public virtual ICollection<EventTag> EventTags { get; set; } = [];

}
