using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Domain.Entities.Identity;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Auth.Commands.CreateStudio;

public class CreateStudioCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateStudioCommandHandler> logger, IGenericRepository<StudioProfile> studioRepository, IUserRepository userRepository)
{
   public async Task<Result<Guid>> Handle(CreateStudioCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(CreateStudioCommand));

        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user == null)
        {
            return Result.NotFound("User Not Found");
        }

        if (user.Role != Domain.Enums.UserRole.Studio)
            return Result.Error("User had not selected studio as role.");

        var doesStudioExists = await studioRepository.AnyAsync(st => st.UserId == command.UserId, cancellationToken);
        if (doesStudioExists)
            return Result.Conflict("A studio for this user already exists");

        var studioDetails = new StudioProfile
        {
            StudioName = command.StudioName,
            UserId = command.UserId,
            Description = command.Description,
            Phone = command.PhoneNumber,
            Website = command.Website,
            Location = new Domain.Entities.Common.ValueObjects.Coordinates(command.Latitude, command.Longitude, string.Empty),
            ServiceRadius = new Domain.Entities.Common.ValueObjects.ServiceRadius()
            {
                RadiusType = Domain.Enums.RadiusType.Km,
                Distance = command.ServiceRadius
            },
            MinPrice = command.MinPrice,
            MaxPrice = command.MaxPrice
        };

        studioRepository.Add(studioDetails);

        await unitOfWork.SaveChangesAsync();
        return Result.Success(studioDetails.Id);
    }
}
