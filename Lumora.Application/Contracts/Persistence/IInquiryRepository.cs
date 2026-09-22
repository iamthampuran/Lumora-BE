using Lumora.Application.Features.Consumer.Queries.GetInquiryWidget;
using Lumora.Application.Features.Studio.Queries.GetInquiries;
using Lumora.Application.Features.Studio.Queries.GetInquiryDetails;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Enums;

namespace Lumora.Application.Contracts.Persistence;

public interface IInquiryRepository : IGenericRepository<Inquiry>
{
    Task<IEnumerable<GetInquiryWidgetResponse>> GetInquiryWidgetDetailsAsync(Guid consumerId, CancellationToken cancellationToken);
    Task<GetInquiriesQueryResponse> GetStudioInquiriesAsync(
    Guid studioId,
    InquiryStatus status,
    PaginationOptions pagination,
    InquiryFilterOptions? filters,
    CancellationToken cancellationToken);

    // Add this to the existing interface
    Task<GetInquiryDetailsResponse?> GetStudioInquiryDetailsAsync(Guid studioId, Guid inquiryId, CancellationToken cancellationToken);
}
