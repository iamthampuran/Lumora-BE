using Lumora.Application.Features.Common.Queries.GetTags;
using Lumora.Domain.Entities.Tag;

namespace Lumora.Application.Contracts.Persistence;

public interface ITagRepository : IGenericRepository<Tag>
{
    Task<IEnumerable<GetTagsQueryResponse>> GetTagsAsync(string? searchText, CancellationToken cancellationToken);
}
