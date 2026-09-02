using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Helpers;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Commands.UpdateCover;

public class UpdateCoverCommandHandler(ILogger<UpdateCoverCommandHandler> logger, IMinioService minioService, IStudioRepository studioRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result<string>> Handle(UpdateCoverCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query - {@query}", nameof(UpdateCoverCommand));
        var studio = await studioRepository.GetByIdAsync(command.StudioId);
        if (studio == null)
        {
            return Result.NotFound("Studio not found");
        }

        if (studio.CoverImageUrl != null)
            await minioService.DeleteFileAsync(studio.CoverImageUrl, cancellationToken);

        var result = await minioService.UploadFileAsync(
            command.File,
            MessageConstants.ImageTypes.Cover,
            command.StudioId.ToString(),
            FileServiceHelper.GetFileNameWithExtension("cover.jpg"),
            cancellationToken);

        studio.CoverImageUrl = result.fileKey;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(result.presignedUrl);
    }
}
