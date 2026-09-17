namespace Lumora.Application.Features.Auth.Commands.UpdatePassword;

public record UpdatePasswordCommand(string NewPassword, string CurrentPassword);
