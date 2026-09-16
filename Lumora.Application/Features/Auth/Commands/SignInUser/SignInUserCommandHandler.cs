using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Services;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Auth.Commands.SignInUser;

public class SignInUserCommandHandler(IUserRepository userRepository, ILogger<SignInUserCommandHandler> logger, IUnitOfWork unitOfWork, IAuthService authService, ITwoFactorAuthService twoFactorAuthService)
{
    public async Task<Result<SignInUserResponse>> Handle(SignInUserCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(SignInUserCommand));

        var user = await userRepository.GetFirstAsync(user => user.Email == command.Email, null, [u => u.ConsumerProfile, u => u.StudioProfile, 
        u => u.StudioProfile.Employees, u => u.StudioProfile.Tags, u => u.StudioProfile.PortfolioImages], true , cancellationToken);
        if (user == null)
        {
            return Result.NotFound("User with the email was not found");
        }
        var passwordVerification = authService.VerifyPasswordAsync(command.Password, user.PasswordHash, user.Salt);

        if (!passwordVerification)
        {
            return Result.Error("Invalid password entered");
        }

        if (user.IsTwoFactorEnabled)
        {
            if (string.IsNullOrWhiteSpace(command.TwoFactorCode))
            {
                // Prompt the UI to ask for the 2FA code
                return Result.Success(new SignInUserResponse(null, null, Requires2FA: true));
            }

            // Verify the provided code
            bool isCodeValid = twoFactorAuthService.ValidateCode(user.TwoFactorSecret!, command.TwoFactorCode);

            // Add backup code verification logic here if you want them to be able to use backup codes

            if (!isCodeValid) return Result.Error("Invalid 2FA code.");
        }

        var result = new SignInUserResponse(await authService.GenerateAccessTokenAsync(user), authService.GenerateRefreshTokenAsync(user).refreshToken, false);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result);
    }
}
