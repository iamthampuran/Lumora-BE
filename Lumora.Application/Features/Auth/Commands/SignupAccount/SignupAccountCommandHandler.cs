using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Domain.Entities.Identity;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Auth.Commands.SignupAccount;

public class SignupAccountCommandHandler(ILogger<SignupAccountCommandHandler> logger, IUserRepository userRepository, IUnitOfWork unitOfWork, IAuthService authService)
{
    public async Task<Result<Guid>> Handle(SignupAccountCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        logger.LogInformation("Handling command - {@command}", nameof(SignupAccountCommand));

        var userExist = await userRepository.AnyAsync(u => u.Email == command.Email, cancellationToken);
        if (userExist)
        {
            return Result.Conflict("An account with the same email already exists");
        }

        (var passwordHash, var salt) = authService.HashPasswordAsync(command.Password);

        var newUser = new User
        {
            Email = command.Email,
            PasswordHash = passwordHash,
            Salt = salt,
            Role = command.Role,
            IsActive = true
        };

        userRepository.Add(newUser);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Created(newUser.Id);
    }
}
