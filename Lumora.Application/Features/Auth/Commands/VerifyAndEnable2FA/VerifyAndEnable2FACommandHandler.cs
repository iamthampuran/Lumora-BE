using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Services;
using System.Text.Json;

namespace Lumora.Application.Features.Auth.Commands.VerifyAndEnable2FA;

public class VerifyAndEnable2FACommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, ITwoFactorAuthService twoFactorAuthService)
{
    public async Task<Result<List<string>>> Handle(VerifyAndEnable2FACommand command, CancellationToken cancellationToken)
    {
        var userDetails = unitOfWork.GetCurrentUserDetails();
        if (userDetails is null) return Result<List<string>>.Unauthorized();

        var user = await userRepository.GetByIdAsync(userDetails.UserId, cancellationToken);
        if (user is null) return Result<List<string>>.NotFound("User not found.");

        // Validate the OTP code
        if (!twoFactorAuthService.ValidateCode(command.Secret, command.Code))
            return Result<List<string>>.Error("Invalid authenticator code.");

        // Enable 2FA on the entity
        user.Enable2FA(command.Secret);

        // Generate, serialize, and store backup codes
        var backupCodes = twoFactorAuthService.GenerateBackupCodes();
        user.TwoFactorBackupCodes = JsonSerializer.Serialize(backupCodes);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Return the plain-text backup codes once so the user can download/copy them
        return Result.Success(backupCodes);
    }
}
