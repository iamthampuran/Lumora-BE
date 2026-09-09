namespace Lumora.Application.Features.Studio.Commands.AddPortfolioImage;

public record AddPortfolioImageCommand(Guid StudioId, Stream FileStream, int Order, string? Title, string ContentType);
