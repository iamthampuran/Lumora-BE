using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Features.Consumer.Queries.FindStudios;
using Lumora.Application.Features.Studio.Queries.GetStudioById;
using Lumora.Application.Features.Studio.Queries.GetStudioDetailsById;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Entities.Identity;
using Lumora.Domain.Enums;
using Lumora.Infrastructure.Data;
using Lumora.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class StudioRepository : GenericRepository<StudioProfile>, IStudioRepository
{
    protected new readonly AppDbContext _appDbContext;
    private readonly IMinioService _minioService;

    public StudioRepository(AppDbContext appDbContext, IMinioService minioService) : base(appDbContext)
    {
        _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        _minioService = minioService ?? throw new ArgumentNullException(nameof(minioService));
    }

    public async Task<PaginatedResponse<FindStudiosQueryResponse>> GetRecommendedStudiosAsync(
        Event eventData,
        StudioFilterOptions? filterOptions,
        StudioSortOption sortOption,
        PaginationOptions paginationOptions,
        CancellationToken cancellationToken)
    {
        IQueryable<StudioProfile> query = _appDbContext.StudioProfiles
            .AsNoTracking()
            .Include(s => s.Tags)
            .ThenInclude(st => st.Tag);

        if (filterOptions != null)
        {
            if (filterOptions.MaxDistance != null)
            {
                query = query.Where(s =>
                    CoordinateHelper.CalculateDistance(s.Location, eventData.Location) <= (double)filterOptions.MaxDistance.Value);
            }

            if (filterOptions.MinRatings != null)
            {
                query = query.Where(s => s.AverageRating >= filterOptions.MinRatings);
            }
        }

        var eventTagIds = eventData.EventTags.Select(et => et.TagId).ToList();

        //if (eventTagIds.Count > 0)
        //{
        //    query = query.Where(s => s.Tags.Any(st => eventTagIds.Contains(st.TagId)));
        //}

        var sortedQuery = sortOption switch
        {
            StudioSortOption.Recommended => query.OrderByDescending(s => s.Tags.Count(st => eventTagIds.Contains(st.TagId)))
            .ThenByDescending(s => s.AverageRating),

            StudioSortOption.Nearest => query.OrderBy(s => CoordinateHelper.CalculateDistance(s.Location, eventData.Location)),

            StudioSortOption.HighestRating => query.OrderByDescending(s => s.AverageRating),

            StudioSortOption.NameDescending => query.OrderByDescending(s => s.StudioName),

            StudioSortOption.NameAscending => query.OrderBy(s => s.StudioName),

            StudioSortOption.PriceHighToLow => query.OrderByDescending(s => s.MinPrice),

            StudioSortOption.PriceLowToHigh => query.OrderBy(s => s.MinPrice),

            _ => query.OrderByDescending(s => s.AverageRating)
        };

        var finalQuery = sortedQuery.Select(s => new
        {
            s.Id,
            s.StudioName,
            s.Location,
            AverageRating = s.AverageRating ?? 0,
            s.ReviewCount,
            TagNames = s.Tags.Select(t => t.Tag.Name).ToList(),
            s.CoverImageUrl,
            s.MinPrice
        });

        var pageResult = await finalQuery.ToPaginatedResponseAsync(paginationOptions.PageCount, paginationOptions.PageSize, cancellationToken);

        var finalMappedData = await Task.WhenAll(pageResult.Data.Select(async s =>
        {
            var coverUrl = s.CoverImageUrl != null ? await _minioService.GeneratePresignedUrlAsync(s.CoverImageUrl) : null;
            var distance = (decimal)CoordinateHelper.CalculateDistance(s.Location, eventData.Location);

            return new FindStudiosQueryResponse(
                s.Id,
                s.StudioName,
                distance,
                s.AverageRating,
                s.ReviewCount,
                s.TagNames,
                coverUrl,
                s.MinPrice
                );
        }));

        return new PaginatedResponse<FindStudiosQueryResponse>(finalMappedData, pageResult.TotalPages, pageResult.CurrentPage, pageResult.PageSize);
    }

    //public async Task<GetStudioByIdResponse?> GetStudioDetailsByIdAsync(Guid id)
    //{
    //    var studio = await _appDbContext.StudioProfiles.AsNoTracking().Where(p => p.Id == id)
    //        .Include(p => p.User)
    //        .Include(p => p.Reviews)
    //        .ThenInclude(r => r.Consumer)
    //        .Include(p => p.Location)
    //        .Include(p => p.PortfolioImages)
    //        .Include(p => p.Inquiries)
    //        .Include(p => p.Tags)
    //        .FirstOrDefaultAsync();


    //    return studio != null ? new GetStudioByIdResponse()
    //    {
    //        Identity = new(id, studio.StudioName, studio.Description, studio.LogoUrl, studio.CoverImageUrl),
    //        RatingStats = new(0, studio.Reviews.Count, studio.Employees.Count, studio.Inquiries.Where(i => i.Event.Status == Domain.Enums.EventStatus.Complete).Count()),
    //        PricingDetails = new(studio.MinPrice, studio.MaxPrice),
    //        GeneralInformation = new(new LocationDetails(studio.Location.ToString(), studio.Location.Latitude, studio.Location.Longitude, studio.ServiceRadius.RadiusType.ToString(),
    //        studio.ServiceRadius.Distance), studio.Phone, studio.User.Email),
    //        Tags = studio.Tags.Select(t => new TagDetails(t.Id, t.Name)).ToList(),
    //        PortfolioDetails = studio.PortfolioImages.Select(pi => new PortfolioDetails(pi.Id, pi.ImageUrl, pi.Title, pi.DisplayOrder)).ToList(),
    //        Reviews = studio.Reviews.OrderByDescending(r => r.ModifiedAt).Select(r => new ReviewDetails(r.Id, r.Consumer.FullName, r.Rating, r.Comment, r.ModifiedAt)).ToList()
    //    } : null;

    //}

    public async Task<GetStudioByIdResponse?> GetStudioDetailsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var studio = await _appDbContext.StudioProfiles
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(studio => new
            {
                studio.Id,
                studio.StudioName,
                studio.Description,
                studio.LogoUrl,
                studio.CoverImageUrl,
                AverageRating = studio.Reviews.Select(r => (decimal?)r.Rating).Average() ?? 0m,
                ReviewCount = studio.Reviews.Count(),
                EmployeeCount = studio.Employees.Count(),
                CompletedInquiryCount = studio.Inquiries.Count(i => i.Event.Status == Domain.Enums.EventStatus.Complete),
                studio.MinPrice,
                studio.MaxPrice,
                LocationText = studio.Location.LocationName,
                studio.Location.Latitude,
                studio.Location.Longitude,
                RadiusType = studio.ServiceRadius.RadiusType.ToString(),
                studio.ServiceRadius.Distance,
                studio.Phone,
                Email = studio.User.Email,
                Tags = studio.Tags.Select(st => new { st.Tag.Id, st.Tag.Name }).ToList(),
                PortfolioImages = studio.PortfolioImages
                    .Select(pi => new { pi.Id, pi.ImageUrl, pi.Title, pi.DisplayOrder })
                    .ToList(),
                Reviews = studio.Reviews
                    .OrderByDescending(r => r.ModifiedAt)
                    .Select(r => new { r.Id, ConsumerName = r.Consumer.FullName, r.Rating, r.Comment, r.ModifiedAt })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (studio == null)
        {
            return null;
        }

        var logoUrl = await _minioService.GeneratePresignedUrlAsync(studio.LogoUrl);
        var coverUrl = await _minioService.GeneratePresignedUrlAsync(studio.CoverImageUrl);

        var portfolioDetails = await Task.WhenAll(studio.PortfolioImages.Select(async pi =>
            new PortfolioDetails(
                pi.Id,
                await _minioService.GeneratePresignedUrlAsync(pi.ImageUrl),
                pi.Title,
                pi.DisplayOrder)));

        return new GetStudioByIdResponse
        {
            Identity = new(
                studio.Id,
                studio.StudioName,
                studio.Description,
                logoUrl,
                coverUrl),

            RatingStats = new(
                studio.AverageRating,
                studio.ReviewCount,
                studio.EmployeeCount,
                studio.CompletedInquiryCount),

            PricingDetails = new(
                studio.MinPrice,
                studio.MaxPrice),

            GeneralInformation = new(
                new LocationDetails(
                    studio.LocationText,
                    studio.Latitude,
                    studio.Longitude,
                    studio.RadiusType,
                    studio.Distance),
                studio.Phone,
                studio.Email),

            Tags = studio.Tags
                .Select(t => new TagDetails(t.Id, t.Name))
                .ToList(),

            PortfolioDetails = portfolioDetails.ToList(),

            Reviews = studio.Reviews.OrderByDescending(r => r.ModifiedAt).Take(5)
                .Select(r => new ReviewDetails(r.Id, r.ConsumerName, r.Rating, r.Comment, r.ModifiedAt))
                .ToList()
        };
    }

    public async Task<GetStudioDetailsByIdResponse?> GetStudioDashboardByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var exists = await _appDbContext.StudioProfiles
            .AsNoTracking()
            .AnyAsync(s => s.Id == id, cancellationToken);

        if (!exists)
        {
            return null;
        }

        var utcNow = DateTime.UtcNow;
        var currentMonthStart = new DateTime(utcNow.Year, utcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextMonthStart = currentMonthStart.AddMonths(1);
        var previousMonthStart = currentMonthStart.AddMonths(-1);

        var currentMonthInquiriesCount = await _appDbContext.Inquiries
            .AsNoTracking()
            .CountAsync(i =>
                i.StudioId == id &&
                i.CreatedAt >= currentMonthStart &&
                i.CreatedAt < nextMonthStart,
                cancellationToken);

        var previousMonthInquiriesCount = await _appDbContext.Inquiries
            .AsNoTracking()
            .CountAsync(i =>
                i.StudioId == id &&
                i.CreatedAt >= previousMonthStart &&
                i.CreatedAt < currentMonthStart,
                cancellationToken);

        var percentageIncrease = previousMonthInquiriesCount == 0
            ? (currentMonthInquiriesCount > 0 ? 100m : 0m)
            : Math.Round(
                ((currentMonthInquiriesCount - previousMonthInquiriesCount) / (decimal)previousMonthInquiriesCount) * 100m,
                2,
                MidpointRounding.AwayFromZero);

        var activeInquiriesCount = await _appDbContext.Inquiries
            .AsNoTracking()
            .CountAsync(i =>
                i.StudioId == id &&
                (i.Status == InquiryStatus.Accepted || i.Status == InquiryStatus.Confirmed),
                cancellationToken);

        var pendingInquiriesCount = await _appDbContext.Inquiries
            .AsNoTracking()
            .CountAsync(i =>
                i.StudioId == id &&
                i.Status == InquiryStatus.Submitted,
                cancellationToken);

        var totalRevenueThisMonthDecimal = await _appDbContext.Payments
            .AsNoTracking()
            .Where(p =>
                p.StudioId == id &&
                p.Status == PaymentStatus.Completed &&
                p.CompletedAt >= currentMonthStart &&
                p.CompletedAt < nextMonthStart)
            .SumAsync(p => (decimal?)p.Amount, cancellationToken) ?? 0m;

        var latestThreeInquiries = await _appDbContext.Inquiries
            .AsNoTracking()
            .Where(i => i.StudioId == id)
            .OrderByDescending(i => i.CreatedAt)
            .Take(3)
            .Select(i => new InquiryDetail(
                i.Id,
                i.Consumer.FullName,
                i.Event.EventType.Name,
                i.Event.EventDate))
            .ToListAsync(cancellationToken);

        var ratingAggregate = await _appDbContext.Reviews
            .AsNoTracking()
            .Where(r => r.StudioId == id)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                AverageRating = g.Average(x => x.Rating),
                ReviewCount = g.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        var latestReview = await _appDbContext.Reviews
            .AsNoTracking()
            .Where(r => r.StudioId == id)
            .OrderByDescending(r => r.ModifiedAt)
            .Select(r => new
            {
                ConsumerName = r.Consumer.FullName,
                r.Rating,
                r.Comment,
                r.ModifiedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        var galleries = await _appDbContext.Galleries
            .AsNoTracking()
            .Where(g =>
                g.Inquiry.StudioId == id &&
                (g.GalleryStatus == GalleryStatus.Uploaded ||
                 g.GalleryStatus == GalleryStatus.UnderReview ||
                 g.GalleryStatus == GalleryStatus.ChangesRequested))
            .OrderByDescending(g => g.ModifiedAt)
            .Take(2)
            .Select(g => new
            {
                g.Id,
                EventName = g.Inquiry.Event.Title,
                Status = g.GalleryStatus.ToString(),
                g.GalleryCover
            })
            .ToListAsync(cancellationToken);

        var galleryDetails = await Task.WhenAll(galleries.Select(async g =>
            new GalleryDetail(
                g.Id,
                g.EventName,
                g.Status,
                string.IsNullOrWhiteSpace(g.GalleryCover)
                    ? string.Empty
                    : await _minioService.GeneratePresignedUrlAsync(g.GalleryCover))));

        var statsData = new StatsData(
            currentMonthInquiriesCount,
            percentageIncrease,
            activeInquiriesCount,
            pendingInquiriesCount,
            Convert.ToInt64(decimal.Round(totalRevenueThisMonthDecimal, 0, MidpointRounding.AwayFromZero)));

        var ratingDetails = new RatingDetails(
            ratingAggregate?.AverageRating ?? 0m,
            ratingAggregate?.ReviewCount ?? 0m,
            latestReview == null ? null :
            new ReviewerDetails(latestReview.Rating, latestReview.Comment, DateOnly.FromDateTime(latestReview.ModifiedAt), latestReview.ConsumerName));

        return new GetStudioDetailsByIdResponse(
            statsData,
            latestThreeInquiries,
            ratingDetails,
            galleryDetails);
    }
}
