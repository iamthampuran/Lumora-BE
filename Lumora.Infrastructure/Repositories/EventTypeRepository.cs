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

    public async Task<IEnumerable<GetEventTypesQueryResponse>> GetEventTypes(string? searchText, bool includeOnlyPredefined = true, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.EventTypes.AsQueryable().AsNoTracking();
        if (searchText != null)
        {
            query = query.Where(et => et.Name.Contains(searchText));
        }

        if (includeOnlyPredefined)
        {
            query = query.Where(et => et.IsPredefined);
        }

        return await query.OrderBy(et => et.CreatedAt).Select(et => new GetEventTypesQueryResponse(et.Id, et.Name)).ToListAsync(cancellationToken);
    }
}
