using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Consumer.Queries.GetEventForEdit;

public class GetEventForEditQueryHandler(ILogger<GetEventForEditQueryHandler> logger, IEventRepository eventRepository, IUnitOfWork unitOfWork) 
{
    public async Task<Result<GetEventForEditQueryResponse>> Handle(GetEventForEditQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetEventForEditQuery for Event ID: {EventId}", query.Id);
        var existingEvent = await eventRepository.GetFirstAsync(e => e.Id == query.Id, null, includes: [e => e.EventTags, e => e.EventType], false, cancellationToken);
        if (existingEvent is null)
        {
            logger.LogWarning("Event with ID {EventId} not found.", query.Id);
            return Result.NotFound();
        }

        var details = unitOfWork.GetCurrentUserDetails();

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
            existingEvent.EventTags.Where(et => et.IsActive).Select(et => et.Tag).ToList()
        );
        return Result.Success(response);
    }
}
