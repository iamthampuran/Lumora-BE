using Lumora.Domain.Entities.Common.ValueObjects;
using Lumora.Domain.Entities.Tag;

namespace Lumora.Application.Features.Consumer.Queries.GetEventForEdit;

public record GetEventForEditQueryResponse(Guid Id,
    Guid ConsumerId,
    string Title,
    DateOnly EventDate,
    Coordinates Location,
    Guid EventTypeId,
    decimal Budget,
    decimal Duration,
    string? SpecialRequirements,
    IEnumerable<Tag> Tags);

