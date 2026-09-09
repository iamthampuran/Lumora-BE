using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Studio;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Commands.AddPortfolioImage;

public class AddPortfolioImageCommandHandler(ILogger<AddPortfolioImageCommandHandler> logger, IStudioRepository studioRepository, 
    IGenericRepository<PortfolioImage> portfolioImageRepository, IUnitOfWork unitOfWork, IMinioService minioService)
{
    public async Task<Result<Guid>> Handle(AddPortfolioImageCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(AddPortfolioImageCommand));
        var studio = await studioRepository.GetByIdAsync(command.StudioId);
        if (studio == null)
        {
            return Result.NotFound("Studio with the id was not found");
        }

        var imageUploaded = await minioService.UploadFileAsync(command.FileStream, MessageConstants.ImageTypes.Portfolio, command.StudioId.ToString(), 
            command.Title ?? $"PortfolioImage_{command.Order}", cancellationToken);

        var portfolioImage = new PortfolioImage()
        {
            StudioId = command.StudioId,
            ImageUrl = imageUploaded.fileKey,
            Title = command.Title,
            DisplayOrder = command.Order
        };

        portfolioImageRepository.Add(portfolioImage);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(portfolioImage.Id);
    }
}
