using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Features.Auth.Commands.SignInUser;
using Lumora.Application.Services;

namespace Lumora.Application.Features.Auth.Commands.Verify2FALogin;

public class Verify2FALoginCommandHandler(
    IUserRepository userRepository,
    IAuthService authService,
    ITwoFactorAuthService twoFactorAuthService,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<SignInUserResponse>> Handle(Verify2FALoginCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetFirstAsync(u => u.Email == command.Email, null, [u => u.ConsumerProfile, u => u.StudioProfile], true, cancellationToken);
        if (user == null) return Result.NotFound("User not found");

        if (!user.IsTwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
            return Result.Error("2FA is not enabled for this user.");

        var isTotpValid = twoFactorAuthService.ValidateCode(user.TwoFactorSecret, command.Code);
        var isBackupCodeValid = false;

        if (!isTotpValid)
        {
            isBackupCodeValid = twoFactorAuthService.TryConsumeBackupCode(command.Code, user.TwoFactorBackupCodes, out var updatedJson);
            if (isBackupCodeValid)
            {
                user.TwoFactorBackupCodes = updatedJson;
            }
        }

        if (!isTotpValid && !isBackupCodeValid)
            return Result.Error("Invalid authenticator/backup code.");

        var result = new SignInUserResponse(
            await authService.GenerateAccessTokenAsync(user),
            authService.GenerateRefreshTokenAsync(user).refreshToken,
            false
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(result);
    }
}