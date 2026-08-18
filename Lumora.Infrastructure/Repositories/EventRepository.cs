using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Features.Consumer.Queries.GetDashboardTable;
using Lumora.Application.Features.Consumer.Queries.GetEventById;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Enums;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class EventRepository : GenericRepository<Event>, IEventRepository
{
    protected new readonly AppDbContext _appDbContext;
    private readonly IMinioService _minioService;
    public EventRepository(AppDbContext appDbContext, IMinioService minioService) : base(appDbContext)
    {
        _appDbContext = appDbContext;
        _minioService = minioService;
    }

    public async Task<GetDashboardTableQueryResponse> GetConsumerEventsAsync(Guid id, EventStatus status, PaginationOptions paginationOptions, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Events.AsNoTracking().Where(e => e.ConsumerId == id);

        var createdEventsCount = await query.CountAsync(e => e.Status == EventStatus.Created, cancellationToken);
        var completedEventsCount = await query.CountAsync(e => e.Status == EventStatus.Complete, cancellationToken);
        var activeEventsCount = await query.CountAsync(e => e.Status != EventStatus.Created  && e.Status != EventStatus.Complete, cancellationToken);


        var newQuery = query.Where(e => status == EventStatus.InProgress ? (e.Status != EventStatus.Created && e.Status != EventStatus.Complete) : (e.Status == status))
            .Skip((paginationOptions.PageCount - 1)* paginationOptions.PageSize)
            .Take(paginationOptions.PageSize);

        var result = await newQuery.OrderByDescending(e => e.ModifiedAt).Select(e => new EventDetails(
                e.Id,
                e.Title,
                e.EventDate,
                e.Location,
                e.Duration,
                e.ModifiedAt
            )).ToListAsync(cancellationToken);


        return new GetDashboardTableQueryResponse(createdEventsCount, completedEventsCount, activeEventsCount, result);

    }

    public async Task<GetEventByIdQueryResponse?> GetEventDetailsAsync(Guid id, CancellationToken cancellationToken, bool disableTracking = true)
    {
        var query = _appDbContext.Events.AsQueryable();

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        query = query.Where(e => e.Id == id);

        var inquiryRows = await query
            .SelectMany(e => e.Inquiries)
            .Select(i => new
            {
                i.Id,
                i.Studio.StudioName,
                i.Studio.LogoUrl,
                i.Status,
                i.QuotedAmount,
                i.ModifiedAt
            })
            .ToListAsync(cancellationToken);

        var inquiryDetails = await Task.WhenAll(inquiryRows.Select(async i =>
        {
            var profileUrl = i.LogoUrl is null
                ? string.Empty
                : await _minioService.GeneratePresignedUrlAsync(i.LogoUrl, 180);

            return new InquiryDetails(
                i.Id,
                i.StudioName,
                profileUrl,
                i.Status.ToString(),
                i.QuotedAmount,
                i.ModifiedAt);
        }));

        var eventDetails = await query
            .Select(e => new EventInformationDetails(
                e.EventType.Name,
                e.Duration,
                e.Budget,
                e.EventTags.Select(t => t.Tag.Name).ToList(),
                e.SpecialRequirements,
                e.Title,
                e.EventDate,
                e.Location))
            .FirstOrDefaultAsync(cancellationToken);

        if (eventDetails is null)
        {
            return null;
        }

        return new GetEventByIdQueryResponse(inquiryDetails, eventDetails);
    }
}
