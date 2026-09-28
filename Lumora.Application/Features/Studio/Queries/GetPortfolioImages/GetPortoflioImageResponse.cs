namespace Lumora.Application.Features.Studio.Queries.GetPortfolioImages;

public record GetPortoflioImageResponse(Guid id, string imageUrl, string? title, int displayOrder);