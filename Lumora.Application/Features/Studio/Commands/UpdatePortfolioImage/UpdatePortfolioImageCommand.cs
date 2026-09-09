namespace Lumora.Application.Features.Studio.Commands.UpdatePortfolioImage;

public record UpdatePortfolioImageCommand(string? Title, Stream? File, string? ContentType, int? Order, Guid ImageId, bool IsDeleted);