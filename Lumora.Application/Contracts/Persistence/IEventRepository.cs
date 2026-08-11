using Lumora.Application.Features.Consumer.Queries.GetDashboardTable;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Enums;

namespace Lumora.Application.Contracts.Persistence;

public interface IEventRepository : IGenericRepository<Event>
{
    Task<GetDashboardTableQueryResponse> GetConsumerEventsAsync(Guid id, EventStatus status, PaginationOptions paginationOptions, CancellationToken cancellationToken = default);
}
