using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Auth.Commands.SignInUser;

public class SignInUserCommandHandler(IUserRepository userRepository, ILogger<SignInUserCommandHandler> logger, IUnitOfWork unitOfWork, IAuthService authService)
{
    public async Task<Result<SignInUserResponse>> Handle(SignInUserCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(SignInUserCommand));

        var user = await userRepository.GetFirstAsync(user => user.Email == command.Email, cancellationToken);
        if (user == null)
        {
            return Result.NotFound("User with the email was not found");
        }
        var passwordVerification = authService.VerifyPasswordAsync(command.Password, user.PasswordHash, user.Salt);

        if (!passwordVerification)
        {
            return Result.Error("Invalid password entered");
        }

        var result = new SignInUserResponse(authService.GenerateAccessTokenAsync(user), authService.GenerateRefreshTokenAsync(user).refreshToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result);
    }
}
