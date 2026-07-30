using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Identity;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Consumer.Commands.AddProfilePicture;

public class AddProfilePictureCommandHandler(ILogger<AddProfilePictureCommandHandler> logger, IUnitOfWork unitOfWork, IGenericRepository<ConsumerProfile> consumerProfileRepository,
    IMinioService minioService)

{
    public async Task<Result<string>> Handle(AddProfilePictureCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(AddProfilePictureCommand));
        var consumer = await consumerProfileRepository.GetByIdAsync(command.consumerId, cancellationToken);
        if (consumer == null)
        {
            return Result.NotFound();
        }

        if (consumer.PhotoUrl != null)
            await minioService.DeleteFileAsync(consumer.PhotoUrl, cancellationToken);

        var result = await minioService.UploadFileAsync(
            command.fileStream,
            MessageConstants.ImageTypes.Avatar,
            command.consumerId.ToString(),
            FileServiceHelper.GetFileNameWithExtension("avatar.jpg"),
            cancellationToken);

        consumer.PhotoUrl = result.fileKey;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(result.presignedUrl);
    }
}
