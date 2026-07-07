using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Features.Studio.Queries.GetStudioById;
using Lumora.Domain.Entities.Identity;
using Lumora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class StudioRepository : GenericRepository<StudioProfile>, IStudioRepository
{
    protected new readonly AppDbContext _appDbContext;

    public StudioRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext)); 
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
                    0,
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
                    .Select(t => new TagDetails(
                        t.Id,
                        t.Name))
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
