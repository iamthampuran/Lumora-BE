using Lumora.Application.Features.Consumer.Queries.GetDashboardTable;
using Lumora.Application.Features.Consumer.Queries.GetEventById;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Enums;

namespace Lumora.Application.Contracts.Persistence;

public interface IEventRepository : IGenericRepository<Event>
{
    Task<PaginatedResponse<EventDetails>> GetConsumerEventsAsync(Guid id, EventStatus status, PaginationOptions paginationOptions, string? searchText, EventFilterOptions? eventFilterOptions, CancellationToken cancellationToken = default);
    Task<GetEventByIdQueryResponse?> GetEventDetailsAsync(Guid id, CancellationToken cancellationToken, bool disableTracking = true);
}
