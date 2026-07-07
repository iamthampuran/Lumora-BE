namespace Lumora.Application.Features.Studio.Queries.GetStudioById;

public class GetStudioByIdResponse
{
    public StudioIdentityResponse Identity { get; init; }
    public RatingStats RatingStats { get; init; }
    public PricingDetails PricingDetails { get; init; }
    public GeneralInformation GeneralInformation { get; init; }
    public List<TagDetails> Tags { get; set; } = [];
    public List<PortfolioDetails> PortfolioDetails { get; set; } = [];
    public List<ReviewDetails> Reviews { get; set; } = [];

}

public record StudioIdentityResponse(Guid id, string studioName, string? about, string? logoUrl, string? coverImageUrl);
public record RatingStats(decimal averageRating, int reviewCount, int teamMembersCount, int projectsCompleted);
public record PricingDetails(decimal minPrice, decimal maxPrice);
public record GeneralInformation(LocationDetails locationDetails, string phone, string email);
public record LocationDetails(string city, double latitude, double longitude, string serviceRadiusType, double? distance);
public record TagDetails(Guid id, string name);
public record PortfolioDetails(Guid id, string imageUrl, string title, int displayOrder);
public record ReviewDetails(Guid id, string reviewerName, decimal rating, string? comment, DateTime date);
