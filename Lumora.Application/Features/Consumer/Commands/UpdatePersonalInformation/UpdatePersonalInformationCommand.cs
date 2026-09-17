namespace Lumora.Application.Features.Consumer.Commands.UpdatePersonalInformation;

public record UpdatePersonalInformationCommand(string FullName, string? PhoneNumber, string? Bio);