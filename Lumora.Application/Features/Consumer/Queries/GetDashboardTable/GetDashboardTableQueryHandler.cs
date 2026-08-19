using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Consumer.Queries.GetDashboardTable;

public class GetDashboardTableQueryHandler(ILogger<GetDashboardTableQueryHandler> logger, IEventRepository eventRepository)
{
    public async Task<Result<GetDashboardTableQueryResponse>> Handle(GetDashboardTableQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(GetDashboardTableQuery));

        var createdCount = await eventRepository.CountAsync(e => e.Status == Domain.Enums.EventStatus.Created && e.ConsumerId == request.Id);
        var completedCount = await eventRepository.CountAsync(e => e.Status == Domain.Enums.EventStatus.Complete && e.ConsumerId == request.Id);
        var activeCount = await eventRepository.CountAsync(e => e.Status != Domain.Enums.EventStatus.Created && e.Status != Domain.Enums.EventStatus.Complete && e.ConsumerId == request.Id);

        var response = await eventRepository.GetConsumerEventsAsync(request.Id, request.Status, request.PaginationOptions, request.SearchText, cancellationToken);
        return Result.Success(new GetDashboardTableQueryResponse(createdCount, completedCount, activeCount, response));
    }
}
