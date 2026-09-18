using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Features.Consumer.Queries.GetInquiryWidget;
using Lumora.Application.Features.Studio.Queries.GetInquiries;
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
}
