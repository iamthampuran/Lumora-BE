using Lumora.Domain.Entities.Common;
using Lumora.Domain.Entities.Common.ValueObjects;
using Lumora.Domain.Entities.Event.ValueObjects;
using Lumora.Domain.Entities.Identity;
using Lumora.Domain.Entities.Reviews;
using Lumora.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumora.Domain.Entities.Event;

public class Event : BaseEntity
{
    public Guid ConsumerId { get; private set; }
    public Guid? SelectedStudioId { get; set; }
    public string Title { get; set; } = null!;
    public DateOnly EventDate {  get; set; }
    public EventCategory EventCategory {
        get => _eventCategoryType != null
            ? new EventCategory(_eventCategoryType, _eventCategoryValue)
            : null;
        set
        {
            _eventCategoryType = value?.Type;
            _eventCategoryValue = value?.Value;
        }
    }
    public Coordinates Location { get; private set; }
    public decimal Budget { get; set; }
    public decimal Duration { get; private set; } //in hours
    public string? PhotographyStyle {  get; set; }
    public string? SpecialRequirements { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Created;

    private string _eventCategoryType;
    private string _eventCategoryValue;

    public void SyncCategoryToDatabase()
    {
        if (EventCategory != null)
        {
            _eventCategoryType = EventCategory.Type;
            _eventCategoryValue = EventCategory.Value;
        }
    }

    //navigation properties

    public virtual ConsumerProfile Consumer { get; set; } = null!;
    public virtual StudioProfile? SelectedStudio { get; set; }
    public virtual ICollection<Inquiry> Inquiries { get; set; } = [];
    public virtual ICollection<Review> Reviews { get; set; } = [];

}
