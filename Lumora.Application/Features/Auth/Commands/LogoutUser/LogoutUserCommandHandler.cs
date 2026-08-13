using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Auth.Commands.LogoutUser;

public class LogoutUserCommandHandler(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<LogoutUserCommandHandler> logger)
{
    public async Task<Result<int>> Handle(LogoutUserCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling command - {@command}", nameof(LogoutUserCommandHandler));
        bool userExists = await userRepository.AnyAsync(u => u.Id == command.UserId);
        if ( !userExists)
        {
            return Result.NotFound("User with the Id not found");
        }

        var refreshTokensDeleted = await refreshTokenRepository.RemoveRangeAsync(rt => rt.UserId == command.UserId && rt.RevokedAt == null, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(refreshTokensDeleted);
    }
}
