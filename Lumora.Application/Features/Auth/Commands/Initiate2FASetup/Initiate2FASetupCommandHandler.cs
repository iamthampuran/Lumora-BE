using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Services;

namespace Lumora.Application.Features.Auth.Commands.Initiate2FASetup;

public class Initiate2FASetupCommandHandler(IUnitOfWork unitOfWork, ITwoFactorAuthService twoFactoAuthService, IUserRepository userRepository, IAuthService authService)
{
    public async Task<Result<Initiate2FASetupResponse>> Handle(Initiate2FASetupCommand command, CancellationToken cancellationToken)
    {
        var userDetails = unitOfWork.GetCurrentUserDetails();
        if (userDetails == null)
            return Result.Unauthorized();

        var user = await userRepository.GetByIdAsync(userDetails.UserId, cancellationToken);
        if (user == null) 
            return Result.NotFound();

        if (user.IsTwoFactorEnabled)
            return Result.Conflict("2FA is already enabled");

        if (!authService.VerifyPasswordAsync(command.Password, user.PasswordHash, user.Salt))
            return Result.Error("Invalid password entered");

        var setupInfo = twoFactoAuthService.GenerateSetupInfo(user.Email);
        return Result.Success(new Initiate2FASetupResponse(setupInfo.secret, setupInfo.qrCodeUri));
    }
}
