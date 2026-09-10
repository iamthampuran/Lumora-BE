namespace Lumora.Application.Features.Studio.Queries.GetStudioDetailsById;

public record GetStudioDetailsByIdResponse(StatsData StatsData, IEnumerable<InquiryDetail> InquiryDetails, RatingDetails RatingDetails, IEnumerable<GalleryDetail> GalleryDetails);

public record StatsData(int InquiriesCount, decimal PercentageIncrease, int ActiveInquiriesCount, int PendingInquiriesCount, long TotalRevenueThisMonth);
public record InquiryDetail(Guid InquiryId, string CreatedUser, string EventType, DateOnly EventDate);
public record RatingDetails(decimal AverageRating, decimal ReviewCount, ReviewerDetails? ReviewerDetails);
public record ReviewerDetails(decimal RecentReview, string? ReviewComment, DateOnly ReviewDate, string RecentReviewUser);
public record GalleryDetail(Guid Id, string EventName, string Status, string GalleryCoverUrl);