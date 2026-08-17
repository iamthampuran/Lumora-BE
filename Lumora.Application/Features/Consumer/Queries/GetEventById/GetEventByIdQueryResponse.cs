using Lumora.Domain.Entities.Common.ValueObjects;

namespace Lumora.Application.Features.Consumer.Queries.GetEventById;

public record GetEventByIdQueryResponse(IEnumerable<InquiryDetails> InquiryDetails, EventInformationDetails EventInformationDetails);

public record InquiryDetails(Guid Id, string StudioName, string ProfileUrl, string InquiryStatus, decimal? Amount, DateTime LastUpdated);
public record EventInformationDetails(string Category, decimal Duration, decimal Budget, List<string> Tags, string? AdditionalInformation, string Title, DateOnly EventDate, Coordinates Location);
