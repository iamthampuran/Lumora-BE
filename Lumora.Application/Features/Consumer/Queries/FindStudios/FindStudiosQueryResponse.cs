namespace Lumora.Application.Features.Consumer.Queries.FindStudios;

public record FindStudiosQueryResponse(Guid studioId, decimal distance, decimal avgRating, int reviewCount, List<string> tags, string? coverUrl, decimal startingPrice);
