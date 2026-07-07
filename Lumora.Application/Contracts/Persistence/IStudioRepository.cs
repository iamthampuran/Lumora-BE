using Lumora.Application.Features.Studio.Queries.GetStudioById;
using Lumora.Domain.Entities.Identity;

namespace Lumora.Application.Contracts.Persistence;

public interface IStudioRepository : IGenericRepository<StudioProfile>
{
    Task<GetStudioByIdResponse?> GetStudioDetailsByIdAsync(Guid id, CancellationToken cancellationToken);
}
