namespace Lumora.Application.Features.Auth.Commands.CreateStudio;

public record CreateStudioCommand(string StudioName, string? Description, string Phone, string? Website, double ServiceRadius, decimal MinPrice, 
    decimal MaxPrice, double Latitude, double Longitude, Guid UserId);

