using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Features.Consumer.Queries.GetInquiryWidget;
using Lumora.Domain.Entities.Event;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class InquiryRepository : GenericRepository<Inquiry>, IInquiryRepository
{
    protected new readonly AppDbContext _appDbContext;
    public InquiryRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<GetInquiryWidgetResponse>> GetInquiryWidgetDetailsAsync(Guid consumerId, CancellationToken cancellationToken)
    {
        var inquiries = await _appDbContext.Inquiries.Where(i => i.ConsumerId == consumerId)
            .Select(i => new GetInquiryWidgetResponse(i.Id, i.Event.Title, i.Studio.StudioName, i.Event.EventDate, i.ModifiedAt))
            .ToListAsync(cancellationToken);

        return inquiries.AsEnumerable();
    }
}
