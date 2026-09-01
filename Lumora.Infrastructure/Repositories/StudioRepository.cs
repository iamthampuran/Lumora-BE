using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Features.Consumer.Queries.FindStudios;
using Lumora.Application.Features.Studio.Queries.GetStudioById;
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

    public async Task<PaginatedResponse<FindStudiosQueryResponse>> GetRecommendedStudiosAsync(Event eventData, StudioFilterOptions? filterOptions, StudioSortOption sortOption, PaginationOptions paginationOptions, CancellationToken cancellationToken)
    {
        IQueryable<StudioProfile> query = _appDbContext.StudioProfiles.AsNoTracking().Include(s => s.Tags);

        if (filterOptions != null)
        {
            if (filterOptions.MaxDistance != null)
            {
                query.Where(s => CoordinateHelper.CalculateDistance(s.Location, eventData.Location) <= s.ServiceRadius.Distance);
            }

            if (filterOptions.MinRatings != null)
            {
                query.Where(s => s.AverageRating >= filterOptions.MinRatings);
            }
        }

        var eventTagIds = eventData.EventTags.Select(s => s.Id).ToList();

        if (eventTagIds.Count > 0)
        {
            query = query.Where(s => s.Tags.Any(st => eventTagIds.Contains(st.TagId)));
        }

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
            s.Location,
            AverageRating = s.AverageRating ?? 0,
            s.ReviewCount,
            TagNames = s.Tags.Select(t => t.Tag.Name).ToList(),
            s.CoverImageUrl,
            s.StartingPrice
        });

        var pageResult = await finalQuery.ToPaginatedResponseAsync(paginationOptions.PageCount, paginationOptions.PageSize, cancellationToken);

        var finalMappedData = await Task.WhenAll(pageResult.Data.Select(async s =>
        {
            var coverUrl = s.CoverImageUrl != null ? await _minioService.GeneratePresignedUrlAsync(s.CoverImageUrl) : null;
            var distance = (decimal)CoordinateHelper.CalculateDistance(s.Location, eventData.Location);

            return new FindStudiosQueryResponse(
                s.Id,
                distance,
                s.AverageRating,
                s.ReviewCount,
                s.TagNames,
                coverUrl,
                s.StartingPrice
                );
        }));

        return new PaginatedResponse<FindStudiosQueryResponse>(finalMappedData, pageResult.TotalPages, pageResult.PageCount, pageResult.PageSize);
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
        return await _appDbContext.StudioProfiles
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(studio => new GetStudioByIdResponse
            {
                Identity = new(
                    studio.Id,
                    studio.StudioName,
                    studio.Description,
                    studio.LogoUrl,
                    studio.CoverImageUrl),

                RatingStats = new(
                    studio.Reviews.Average(r => r.Rating),
                    studio.Reviews.Count(),
                    studio.Employees.Count(),
                    studio.Inquiries.Count(i =>
                        i.Event.Status == Domain.Enums.EventStatus.Complete)),

                PricingDetails = new(
                    studio.MinPrice,
                    studio.MaxPrice),

                GeneralInformation = new(
                    new LocationDetails(
                        studio.Location.ToString(),
                        studio.Location.Latitude,
                        studio.Location.Longitude,
                        studio.ServiceRadius.RadiusType.ToString(),
                        studio.ServiceRadius.Distance),
                        studio.Phone,
                        studio.User.Email),

                Tags = studio.Tags
                    .Select(st => new TagDetails(
                        st.Tag.Id,
                        st.Tag.Name))
                    .ToList(),

                PortfolioDetails = studio.PortfolioImages
                    .Select(pi => new PortfolioDetails(
                        pi.Id,
                        pi.ImageUrl,
                        pi.Title,
                        pi.DisplayOrder))
                    .ToList(),

                Reviews = studio.Reviews
                    .OrderByDescending(r => r.ModifiedAt)
                    .Select(r => new ReviewDetails(
                        r.Id,
                        r.Consumer.FullName,
                        r.Rating,
                        r.Comment,
                        r.ModifiedAt))
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
