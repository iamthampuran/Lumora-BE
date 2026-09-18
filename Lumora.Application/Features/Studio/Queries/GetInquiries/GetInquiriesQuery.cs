using Lumora.Application.Helpers;
using Lumora.Domain.Enums;

namespace Lumora.Application.Features.Studio.Queries.GetInquiries;

public record GetInquiriesQuery(InquiryStatus Status, PaginationOptions PaginationOptions, InquiryFilterOptions? FilterOptions);

public record InquiryFilterOptions(
    IEnumerable<Guid>? EventTypeIds,
    DateOnly? StartDate,
    DateOnly? EndDate,
    string? Location,
    decimal? MinAmount,
    decimal? MaxAmount
);