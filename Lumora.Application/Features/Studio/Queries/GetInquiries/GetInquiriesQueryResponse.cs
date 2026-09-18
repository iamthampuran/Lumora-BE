using Lumora.Application.Helpers;

namespace Lumora.Application.Features.Studio.Queries.GetInquiries;

public record GetInquiriesQueryResponse(int NewCount, int AcceptedCount, int ConfirmedCount, int RejectedCount, PaginatedResponse<StudioInquiryDetails> Inquiries);

public record StudioInquiryDetails(Guid InquiryId, string ClientName, string? ClientAvatarUrl, string EventType, string EventTitle, DateOnly EventDate, string Location, decimal? QuotedAmount,
    DateTime ReceivedAt);