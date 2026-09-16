using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Services;

namespace Lumora.Application.Features.Auth.Commands.VerifyAndEnable2FA;

public class VerifyAndEnable2FACommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, ITwoFactorAuthService twoFactorAuthService)
{
    public async Task<Result<List<string>>> Handle(VerifyAndEnable2FACommand command, CancellationToken cancellationToken)
    {
        var userDetails = unitOfWork.GetCurrentUserDetails();
        if (userDetails is null) return Result<List<string>>.Unauthorized();

        var user = await userRepository.GetByIdAsync(userDetails.UserId, cancellationToken);
        if (user is null) return Result<List<string>>.NotFound("User not found.");

        if (!twoFactorAuthService.ValidateCode(command.Secret, command.Code))
            return Result<List<string>>.Error("Invalid authenticator code.");

        user.Enable2FA(command.Secret);

        var backupCodes = twoFactorAuthService.GenerateBackupCodes();
        user.TwoFactorBackupCodes = twoFactorAuthService.HashBackupCodes(backupCodes);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(backupCodes);
    }
}