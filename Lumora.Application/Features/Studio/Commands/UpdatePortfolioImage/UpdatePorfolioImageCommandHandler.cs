using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Studio;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Commands.UpdatePortfolioImage;

public class UpdatePorfolioImageCommandHandler(ILogger<UpdatePorfolioImageCommandHandler> logger, IGenericRepository<PortfolioImage> imageRepository, IUnitOfWork unitOfWork,
    IMinioService minioService)
{
    public async Task<Result<Guid>> Handle(UpdatePortfolioImageCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(UpdatePortfolioImageCommand));

        var image = await imageRepository.GetByIdAsync(command.ImageId, cancellationToken);
        if (image == null)
        {
            return Result.NotFound("Image not found");
        }

        if (image.Title != command.Title)
            image.Title = command.Title;

        if (command.File != null)
        {
            await minioService.DeleteFileAsync(image.ImageUrl, cancellationToken);
            var uploadResult = await minioService.UploadFileAsync(command.File, MessageConstants.ImageTypes.Portfolio, image.StudioId.ToString(),
            command.Title ?? $"PortfolioImage_{command.Order}", cancellationToken);
            image.ImageUrl = uploadResult.fileKey;
        }

        image.IsActive = !command.IsDeleted;

        if (command.Order != null)
            image.DisplayOrder = command.Order.Value;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(image.Id);
    }
}
