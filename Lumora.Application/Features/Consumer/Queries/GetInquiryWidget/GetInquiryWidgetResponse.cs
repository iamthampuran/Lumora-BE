namespace Lumora.Application.Features.Consumer.Queries.GetInquiryWidget;

public record GetInquiryWidgetResponse(Guid InquiryId, string EventName, string StudioName, DateOnly EventDate, DateTime LastModifiedAt);

