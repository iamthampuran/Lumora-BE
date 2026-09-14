using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Enums;

namespace Lumora.Application.Features.Consumer.Commands.UpdateEvent;

public class UpdateEventCommandHandler(IEventRepository eventRepository, IEventTypeRepository eventTypeRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result<Guid>> Handle(UpdateEventCommand command, CancellationToken cancellationToken)
    {
        var existingEvent = await eventRepository.GetFirstAsync(e => e.Id == command.Id, null, includes: [e => e.EventTags, e => e.Inquiries], false, cancellationToken);
        if (existingEvent is null)
        {
            return Result.NotFound();
        }

        if (existingEvent.Inquiries.Any(i => i.Status == InquiryStatus.Accepted))
        {
            return Result.Error("Cannot update event with accepted inquiries.");
        }

        Guid eventCategoryId;
        if (command.EventCategoryId == null) //not an existing event type
        {
            var newEventType = new EventType(command.CustomEventCategory!, false);
            eventCategoryId = newEventType.Id;
            eventTypeRepository.Add(newEventType);
        }
        else
        {
            eventCategoryId = command.EventCategoryId.Value;
        }

        existingEvent.ConsumerId = command.ConsumerId;
        existingEvent.Title = command.Title;
        existingEvent.EventDate = command.EventDate;
        existingEvent.Location = command.Location;
        existingEvent.EventTypeId = eventCategoryId;
        existingEvent.Budget = command.Budget;
        existingEvent.Duration = command.Duration;
        existingEvent.SpecialRequirements = command.SpecialRequirements;

        // Handle EventTags: mark existing tags as inactive if not in command
        var commandTagIds = command.TagIds?.ToHashSet() ?? [];
        foreach (var eventTag in existingEvent.EventTags)
        {
            if (!commandTagIds.Contains(eventTag.TagId))
            {
                eventTag.IsActive = false;
                eventTag.DeletedAt = DateTime.UtcNow;
            }
        }

        // Add new EventTags for tags that are in command but not already associated
        var existingTagIds = existingEvent.EventTags
            .Where(et => et.IsActive)
            .Select(et => et.TagId)
            .ToHashSet();

        foreach (var tagId in commandTagIds)
        {
            if (!existingTagIds.Contains(tagId))
            {
                var newEventTag = new EventTag { EventId = existingEvent.Id, TagId = tagId };
                existingEvent.EventTags.Add(newEventTag);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(existingEvent.Id);

    }
}
