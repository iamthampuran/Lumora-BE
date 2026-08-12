using Lumora.Application.Features.Consumer.Queries.GetInquiryWidget;
using Lumora.Domain.Entities.Event;

namespace Lumora.Application.Contracts.Persistence;

public interface IInquiryRepository : IGenericRepository<Inquiry>
{
    Task<IEnumerable<GetInquiryWidgetResponse>> GetInquiryWidgetDetailsAsync(Guid consumerId, CancellationToken cancellationToken);
}
