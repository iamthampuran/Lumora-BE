using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Features.Consumer.Queries.GetInquiryWidget;
using Lumora.Application.Features.Studio.Queries.GetInquiries;
using Lumora.Application.Features.Studio.Queries.GetInquiryDetails;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Enums;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class InquiryRepository : GenericRepository<Inquiry>, IInquiryRepository
{
    protected new readonly AppDbContext _appDbContext;
    private readonly IMinioService _minioService;
    public InquiryRepository(AppDbContext appDbContext, IMinioService minioService) : base(appDbContext)
    {
        _appDbContext = appDbContext;
        _minioService = minioService;
    }

    public async Task<IEnumerable<GetInquiryWidgetResponse>> GetInquiryWidgetDetailsAsync(Guid consumerId, CancellationToken cancellationToken)
    {
        var inquiries = await _appDbContext.Inquiries.Where(i => i.ConsumerId == consumerId)
            .Select(i => new GetInquiryWidgetResponse(i.Id, i.Event.Title, i.Studio.StudioName, i.Event.EventDate, i.ModifiedAt))
            .ToListAsync(cancellationToken);

        return inquiries.AsEnumerable();
    }

    public async Task<GetInquiriesQueryResponse> GetStudioInquiriesAsync(
        Guid studioId,
        InquiryStatus status,
        PaginationOptions pagination,
        InquiryFilterOptions? filters,
        CancellationToken cancellationToken)
    {
        var baseQuery = _appDbContext.Inquiries
            .Include(i => i.Consumer)
            .Include(i => i.Event).ThenInclude(e => e.EventType)
            .Where(i => i.StudioId == studioId && i.IsActive);

        // Fast counts for the UI tabs
        var newCount = await baseQuery.CountAsync(i => i.Status == InquiryStatus.Submitted, cancellationToken);
        var acceptedCount = await baseQuery.CountAsync(i => i.Status == InquiryStatus.Accepted, cancellationToken);
        var confirmedCount = await baseQuery.CountAsync(i => i.Status == InquiryStatus.Confirmed, cancellationToken);
        var rejectedCount = await baseQuery.CountAsync(i => i.Status == InquiryStatus.Rejected, cancellationToken);

        var query = baseQuery.Where(i => i.Status == status);

        if (filters != null)
        {
            if (filters.EventTypeIds != null && filters.EventTypeIds.Any())
                query = query.Where(i => filters.EventTypeIds.Contains(i.Event.EventTypeId));

            if (filters.StartDate.HasValue)
                query = query.Where(i => i.Event.EventDate >= filters.StartDate.Value);

            if (filters.EndDate.HasValue)
                query = query.Where(i => i.Event.EventDate <= filters.EndDate.Value);

            if (!string.IsNullOrWhiteSpace(filters.Location))
                query = query.Where(i => i.Event.Location.LocationName.ToLower().Contains(filters.Location.ToLower()));

            if (filters.MinAmount.HasValue)
                query = query.Where(i => i.QuotedAmount >= filters.MinAmount.Value);

            if (filters.MaxAmount.HasValue)
                query = query.Where(i => i.QuotedAmount <= filters.MaxAmount.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        // 1. Fetch raw data into memory (Anonymous Type)
        var rawItems = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((pagination.PageCount - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(i => new
            {
                i.Id,
                ConsumerName = i.Consumer.FullName,
                ConsumerPhotoUrl = i.Consumer.PhotoUrl,
                EventTypeName = i.Event.EventType.Name,
                EventTitle = i.Event.Title,
                i.Event.EventDate,
                i.Event.Location.LocationName,
                i.QuotedAmount,
                i.CreatedAt
            }).ToListAsync(cancellationToken);

        // 2. Iterate in-memory to generate Presigned URLs
        var inquiriesList = new List<StudioInquiryDetails>();
        foreach (var item in rawItems)
        {
            string? avatarUrl = null;
            if (!string.IsNullOrWhiteSpace(item.ConsumerPhotoUrl))
            {
                avatarUrl = await _minioService.GeneratePresignedUrlAsync(item.ConsumerPhotoUrl);
            }

            inquiriesList.Add(new StudioInquiryDetails(
                item.Id,
                item.ConsumerName,
                avatarUrl,
                item.EventTypeName,
                item.EventTitle,
                item.EventDate,
                item.LocationName,
                item.QuotedAmount,
                item.CreatedAt
            ));
        }

        var paginatedResponse = new PaginatedResponse<StudioInquiryDetails>(inquiriesList, totalItems, pagination.PageCount, pagination.PageSize);

        return new GetInquiriesQueryResponse(newCount, acceptedCount, confirmedCount, rejectedCount, paginatedResponse);
    }

    public async Task<GetInquiryDetailsResponse?> GetStudioInquiryDetailsAsync(Guid studioId, Guid inquiryId, CancellationToken cancellationToken)
    {
        var inquiry = await _appDbContext.Inquiries
            .AsNoTracking()
            .Where(i => i.Id == inquiryId && i.StudioId == studioId && i.IsActive)
            .Select(i => new
            {
                i.Id,
                i.Status,
                i.CreatedAt,
                i.ModifiedAt,
                i.QuotedAmount,
                Event = new
                {
                    i.Event.Title,
                    Category = i.Event.EventType.Name,
                    i.Event.EventDate,
                    i.Event.Location.LocationName,
                    i.Event.Duration,
                    i.Event.Budget,
                    i.Event.SpecialRequirements,
                    Tags = i.Event.EventTags.Select(t => t.Tag.Name).ToList()
                },
                Consumer = new
                {
                    i.Consumer.Id,
                    i.Consumer.FullName,
                    i.Consumer.User.Email,
                    i.Consumer.Phone,
                    PriorBookings = _appDbContext.Inquiries.Count(prev =>
                        prev.ConsumerId == i.ConsumerId && prev.Status == Domain.Enums.InquiryStatus.Confirmed)
                },
                Payment = _appDbContext.Payments.FirstOrDefault(p => p.InquiryId == i.Id)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (inquiry == null) return null;

        PaymentSummary? paymentSummary = null;
        if (inquiry.Payment != null || inquiry.Status == Domain.Enums.InquiryStatus.Accepted || inquiry.Status == Domain.Enums.InquiryStatus.Confirmed)
        {
            var platformFee = inquiry.QuotedAmount * 0.05m; // 5% Platform Fee
            var total = inquiry.QuotedAmount + platformFee;

            paymentSummary = new PaymentSummary(
                ServiceFee: inquiry.QuotedAmount ?? 0,
                PlatformFee: platformFee ?? 0,
                TotalPaid: total ?? 0,
                TransactionStatus: inquiry.Payment?.Status.ToString() ?? "Awaiting Payment",
                TransactionId: inquiry.Payment?.RazorPayOrderId,
                PaymentMethod: "UPI/Net Banking", // Derived from payment record in full implementation
                PaidAt: inquiry.Payment?.CompletedAt
            );
        }

        return new GetInquiryDetailsResponse(
            inquiry.Id,
            inquiry.Status.ToString(),
            inquiry.CreatedAt,
            inquiry.ModifiedAt,
            inquiry.QuotedAmount ?? 0,
            new EventDetails(
                inquiry.Event.Title ?? "Untitled Event",
                inquiry.Event.Category,
                inquiry.Event.EventDate,
                inquiry.Event.LocationName,
                inquiry.Event.Duration,
                inquiry.Event.Budget,
                inquiry.Event.SpecialRequirements ?? string.Empty,
                inquiry.Event.Tags
            ),
            new ConsumerDetails(
                inquiry.Consumer.Id,
                inquiry.Consumer.FullName,
                inquiry.Consumer.Email ?? string.Empty,
                inquiry.Consumer.Phone ?? string.Empty,
                inquiry.Consumer.PriorBookings,
                inquiry.Consumer.PriorBookings > 0 ? "Elite" : "New Client"
            ),
            paymentSummary
        );
    }
}
