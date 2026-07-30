using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Helpers;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Commands.UpdateLogo;

public class UpdateLogoCommandHandler(ILogger<UpdateLogoCommandHandler> logger, IStudioRepository studioRepository, IMinioService minioService, IUnitOfWork unitOfWork)
{
    public async Task<Result<string>> Handle(UpdateLogoCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(UpdateLogoCommand));
        var studio = await studioRepository.GetByIdAsync(command.StudioId, cancellationToken);
        if (studio == null)
        {
            return Result.NotFound();
        }

        if (studio.LogoUrl != null)
            await minioService.DeleteFileAsync(studio.LogoUrl, cancellationToken);

        var result = await minioService.UploadFileAsync(
            command.File, 
            MessageConstants.ImageTypes.Logo, 
            command.StudioId.ToString(), 
            FileServiceHelper.GetFileNameWithExtension("logo.jpg"), 
            cancellationToken);
            
        studio.LogoUrl = result.fileKey;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(result.presignedUrl);
    }
}
