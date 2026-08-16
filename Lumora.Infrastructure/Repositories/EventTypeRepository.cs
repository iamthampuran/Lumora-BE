using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Features.Common.Queries.GetEventCategories;
using Lumora.Domain.Entities.Event;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class EventTypeRepository : GenericRepository<EventType>, IEventTypeRepository
{
    protected new readonly AppDbContext _appDbContext;
    public EventTypeRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<GetEventTypesQueryResponse>> GetEventTypes(string? searchText, CancellationToken cancellationToken)
    {
        var query = _appDbContext.EventTypes.AsQueryable().AsNoTracking();
        if (searchText != null)
        {
            query = query.Where(x => x.Name.Contains(searchText));
        }

        return await query.Select(x => new GetEventTypesQueryResponse(x.Id, x.Name)).ToListAsync(cancellationToken);
    }
}
