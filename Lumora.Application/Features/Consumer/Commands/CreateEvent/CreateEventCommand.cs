using Lumora.Domain.Entities.Common.ValueObjects;

namespace Lumora.Application.Features.Consumer.Commands.CreateEvent;

public record CreateEventCommand(string title, Guid? eventCategoryId, string? customEventCategory, decimal budget, Coordinates Location, )
