using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Features.Common.Queries.GetTags;
using Lumora.Domain.Entities.Tag;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class TagRepository : GenericRepository<Tag>, ITagRepository
{
    protected new readonly AppDbContext _appDbContext;
    public TagRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<GetTagsQueryResponse>> GetTagsAsync(string? searchText, CancellationToken cancellationToken)
    {
        var tagsQuery = _appDbContext.Tags.AsQueryable().AsNoTracking();

        if (searchText != null)
        {
            tagsQuery = tagsQuery.Where(t => t.Name.Contains(searchText));
        }

        var tagResults = await tagsQuery.OrderBy(t => t.CreatedAt).Select(t => new GetTagsQueryResponse(t.Id, t.Name)).ToListAsync(cancellationToken);
        return tagResults.AsEnumerable();
    }
}
