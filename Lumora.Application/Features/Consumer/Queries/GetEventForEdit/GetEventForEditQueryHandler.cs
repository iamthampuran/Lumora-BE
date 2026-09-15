using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Consumer.Queries.GetEventForEdit;

public class GetEventForEditQueryHandler(
    ILogger<GetEventForEditQueryHandler> logger,
    IEventRepository eventRepository,
    ITagRepository tagRepository, // 1. Inject ITagRepository
    IUnitOfWork unitOfWork)
{
    public async Task<Result<GetEventForEditQueryResponse>> Handle(GetEventForEditQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetEventForEditQuery for Event ID: {EventId}", query.Id);

        // 2. Revert includes to first-level only to avoid the EF Core translation error
        var existingEvent = await eventRepository.GetFirstAsync(
            e => e.Id == query.Id,
            null,
            includes: [e => e.EventTags, e => e.EventType],
            false,
            cancellationToken);

        if (existingEvent is null)
        {
            logger.LogWarning("Event with ID {EventId} not found.", query.Id);
            return Result.NotFound();
        }

        var details = unitOfWork.GetCurrentUserDetails();

        // 3. Extract the active TagIds from the event
        var activeTagIds = existingEvent.EventTags
            .Where(et => et.IsActive)
            .Select(et => et.TagId)
            .ToList();

        // 4. Fetch the actual Tags in a lightweight secondary query
        var tags = await tagRepository.GetAsync(t => activeTagIds.Contains(t.Id), cancellationToken);

        var response = new GetEventForEditQueryResponse(
            existingEvent.Id,
            existingEvent.ConsumerId,
            existingEvent.Title,
            existingEvent.EventDate,
            existingEvent.Location,
            existingEvent.EventTypeId,
            existingEvent.Budget,
            existingEvent.Duration,
            existingEvent.SpecialRequirements,
            tags.ToDictionary(t => t.Id, t => t.Name) // 5. Map safely to dictionary
        );

        return Result.Success(response);
    }
}