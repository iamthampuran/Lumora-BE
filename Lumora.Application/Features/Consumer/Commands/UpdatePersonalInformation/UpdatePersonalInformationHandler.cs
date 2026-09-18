using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Domain.Entities.Identity;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Consumer.Commands.UpdatePersonalInformation;

public class UpdatePersonalInformationHandler(IGenericRepository<ConsumerProfile> consuperProfileRepository, IUnitOfWork unitOfWork, ILogger<UpdatePersonalInformationHandler> logger, 
    ICurrentUserService currentUserService)
{
    public async Task<Result<Guid>> Handle(UpdatePersonalInformationCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(UpdatePersonalInformationCommand));

        var userDetails = currentUserService.GetCurrentUserDetails();

        if (userDetails == null || userDetails.ConsumerId == null)
            return Result.Unauthorized("Current user is not consumer or not found");

        var consumer = await consuperProfileRepository.GetByIdAsync(userDetails.ConsumerId.Value, cancellationToken);
        if (consumer == null)
            return Result.NotFound("Consumer with the id is not found");

        consumer.FullName = command.FullName;
        consumer.Phone = command.PhoneNumber;
        consumer.Bio = command.Bio;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(consumer.Id);
    }
}
