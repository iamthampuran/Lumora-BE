using Ardalis.Result;
using Lumora.Application.Configuration;
using Lumora.Application.Contracts.Persistence;
using Lumora.Domain.Entities.Event;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lumora.Application.Features.Consumer.Queries.GetStudioById;

public class GetStudioByIdQueryHandler(ILogger<GetStudioByIdQueryHandler> logger, IEventRepository eventRepository, IStudioRepository studioRepository)
{
    public async Task<Result<GetStudioByIdResponse>> Handle(GetStudioByIdQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(GetStudioByIdQueryHandler));
        ArgumentNullException.ThrowIfNull(nameof(query));
        bool? isAvailable = null;

        if (query.EventId != null)
        {
            var eventData = await eventRepository.GetByIdAsync(query.EventId.Value, cancellationToken);
            if (eventData is null)
                return Result.NotFound("Event with the id passed is not found");

            // Use EventRepository to check for conflicting bookings
            bool hasEventOnDate = await eventRepository.AnyAsync(
                e => e.SelectedStudioId == query.Id && e.EventDate == eventData.EventDate,
                cancellationToken);

            isAvailable = !hasEventOnDate;
        }

        var studio = await studioRepository.GetStudioDetailsByIdAsync(query.Id, isAvailable, cancellationToken);
        return studio != null ? Result.Success(studio) : Result.NotFound();
    }
}
