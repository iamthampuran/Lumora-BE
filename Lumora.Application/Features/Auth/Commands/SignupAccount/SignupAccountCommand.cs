using Lumora.Domain.Enums;

namespace Lumora.Application.Features.Auth.Commands.SignupAccount;

public record SignupAccountCommand(string Email, string Password, UserRole Role);
