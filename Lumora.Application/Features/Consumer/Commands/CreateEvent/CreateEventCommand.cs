using Lumora.Domain.Entities.Common.ValueObjects;

namespace Lumora.Application.Features.Consumer.Commands.CreateEvent;

public record CreateEventCommand(string Title, Guid? EventCategoryId, string? CustomEventCategory, decimal Budget, Coordinates Location, DateOnly EventDate, decimal Duration, IEnumerable<Guid> TagIds, Guid ConsumerId, string? SpecialRequirements);
