using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Features.Consumer.Queries.GetDashboardTable;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Enums;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class EventRepository : GenericRepository<Event>, IEventRepository
{
    protected new readonly AppDbContext _appDbContext;
    public EventRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<GetDashboardTableQueryResponse> GetConsumerEventsAsync(Guid id, EventStatus status, PaginationOptions paginationOptions, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Events.AsNoTracking().Where(e => e.ConsumerId == id);

        var createdEventsCount = await query.CountAsync(e => e.Status == EventStatus.Created, cancellationToken);
        var completedEventsCount = await query.CountAsync(e => e.Status == EventStatus.Complete, cancellationToken);
        var activeEventsCount = await query.CountAsync(e => e.Status != EventStatus.Created  && e.Status != EventStatus.Complete, cancellationToken);


        var newQuery = query.Where(e => status == EventStatus.InProgress ? (e.Status != EventStatus.Created && e.Status != EventStatus.Complete) : (e.Status == status))
            .Skip((paginationOptions.PageCount - 1)* paginationOptions.PageSize)
            .Take(paginationOptions.PageSize);

        var result = await newQuery.Select(e => new EventDetails(
                e.Id,
                e.Title,
                e.EventDate,
                e.Location,
                e.Duration,
                e.ModifiedAt
            )).ToListAsync(cancellationToken);


        return new GetDashboardTableQueryResponse(createdEventsCount, completedEventsCount, activeEventsCount, result);

    }
}
