using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Identity;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Auth.Commands.CreateConsumer;

public class CreateConsumerCommandHandler(ILogger<CreateConsumerCommandHandler> logger, IUnitOfWork unitOfWork, IGenericRepository<ConsumerProfile> consumerProfileRepository, 
    IUserRepository userRepository, IMinioService minioService)
{
    public async Task<Result<Guid>> Handle(CreateConsumerCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command {@command}", nameof(CreateConsumerCommand));
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user == null)
        {
            return Result.NotFound("User Not Found");
        }

        if (user.Role != Domain.Enums.UserRole.Consumer)
            return Result.Error("User had not selected consumer as role.");

        var doesConsumerExists = await consumerProfileRepository.AnyAsync(cp => cp.UserId == command.UserId, cancellationToken);
        if (doesConsumerExists)
        {
            return Result.Conflict("A consumer with same userId already exists!");
        }

        var consumerProfile = new ConsumerProfile(command.UserId, command.FullName, command.PhoneNumber, null, command.Bio);

        if (command.FileDetails != null)
        {
            var fileResponse = await minioService.UploadFileAsync(command.FileDetails.fileStream, MessageConstants.ImageTypes.Avatar, consumerProfile.Id.ToString(),
                FileServiceHelper.GetFileNameWithExtension("avatar.jpg"), cancellationToken);
            consumerProfile.PhotoUrl = fileResponse.fileKey;
        }

        consumerProfileRepository.Add(consumerProfile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(consumerProfile.Id);
    }
}
