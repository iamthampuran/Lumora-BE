using Lumora.Application.Features.Common.Queries.GetEventCategories;
using Lumora.Domain.Entities.Event;

namespace Lumora.Application.Contracts.Persistence;

public interface IEventTypeRepository : IGenericRepository<EventType>
{
    Task<IEnumerable<GetEventTypesQueryResponse>> GetEventTypes(string? searchText, bool includeOnlyPredefined = true, CancellationToken cancellationToken = default);
}
