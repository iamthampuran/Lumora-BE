using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Consumer.Queries.GetEventById;

public class GetEventByIdQueryHandler (ILogger<GetEventByIdQueryHandler> logger, IEventRepository eventRepository)
{
    public async Task<Result<GetEventByIdQueryResponse>> Handle(GetEventByIdQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(GetEventByIdQuery));
        var response = await eventRepository.GetEventDetailsAsync(query.EventId, cancellationToken);
        if (response == null)
        {
            return Result.NotFound();
        }
        return Result.Success(response);
    }
}
