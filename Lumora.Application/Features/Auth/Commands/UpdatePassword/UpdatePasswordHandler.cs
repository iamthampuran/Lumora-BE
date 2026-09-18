using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Auth.Commands.UpdatePassword;

public class UpdatePasswordHandler(ILogger<UpdatePasswordCommand> logger, IUserRepository userRepository, IUnitOfWork unitOfWork, IAuthService authService, ICurrentUserService currentUserService)
{
    public async Task<Result<Guid>> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(UpdatePasswordCommand));
        var userDetails = currentUserService.GetCurrentUserDetails();
        if (userDetails == null) return Result.Unauthorized("User is not logged in");

        var user = await userRepository.GetByIdAsync(userDetails.UserId, cancellationToken);
        if (user == null) return Result.NotFound("User is not found");

        var isCorrectPassword = authService.VerifyPasswordAsync(command.CurrentPassword, user.PasswordHash, user.Salt);
        if (!isCorrectPassword)
            return Result.Error("The entered current password is wrong");


        var newPasswordDetails = authService.HashPasswordAsync(command.NewPassword);
        user.PasswordHash = newPasswordDetails.passwordHash;
        user.Salt = newPasswordDetails.salt;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(user.Id);
    }
}
