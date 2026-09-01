using Lumora.Application.Features.Consumer.Queries.FindStudios;
using Lumora.Application.Features.Studio.Queries.GetStudioById;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Entities.Identity;
using Lumora.Domain.Enums;

namespace Lumora.Application.Contracts.Persistence;

public interface IStudioRepository : IGenericRepository<StudioProfile>
{
    Task<GetStudioByIdResponse?> GetStudioDetailsByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PaginatedResponse<FindStudiosQueryResponse>> GetRecommendedStudiosAsync(Event eventData, StudioFilterOptions? filterOptions, StudioSortOption sortOption, PaginationOptions paginationOptions, CancellationToken cancellationToken);
}
