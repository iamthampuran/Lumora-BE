using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Entities.Tag;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Consumer.Commands.CreateEvent;

public class CreateEventCommandHandler(ILogger<CreateEventCommandHandler> logger, IEventRepository eventRepository, IGenericRepository<EventType> eventTypeRepository, IGenericRepository<EventTag> eventTagRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result<Guid>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(CreateEventCommand));
        var existingEvent = await eventRepository.AnyAsync(e => e.EventDate == request.EventDate && e.ConsumerId == request.ConsumerId, cancellationToken);
        
        if (existingEvent)
        {
            return Result<Guid>.Error("You already have an event on that day.");
        }

        //var existingEventType = await eventTypeRepository.AnyAsync(et => et.Id == request.EventCategoryId.Value, cancellationToken);
        Guid eventCategoryId;
        if (request.EventCategoryId == null) //not an existing event type
        {
            var newEventType = new EventType(request.CustomEventCategory!, false);
            eventCategoryId = newEventType.Id;
            eventTypeRepository.Add(newEventType);
        }
        else
        {
            eventCategoryId = request.EventCategoryId.Value;
        }


        Event newEvent = new Event()
        {
            ConsumerId = request.ConsumerId,
            Title = request.Title,
            EventDate = request.EventDate,
            Location = request.Location,
            EventTypeId = eventCategoryId,
            Budget = request.Budget,
            Duration = request.Duration,
            SpecialRequirements = request.SpecialRequirements
        };

        eventRepository.Add(newEvent);

        var eventTags = request.TagIds.Select(tagId => new EventTag
        {
            EventId = newEvent.Id,
            TagId = tagId
        });

        eventTagRepository.AddRange(eventTags);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(newEvent.Id);
    }
}
