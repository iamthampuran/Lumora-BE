using Lumora.Domain.Entities.Common.ValueObjects;
using Lumora.Domain.Enums;

namespace Lumora.Application.Features.Consumer.Queries.GetDashboardTable;


public record GetDashboardTableQueryResponse(int CreatedCount, int CompletedCount, int ActiveCount, IEnumerable<EventDetails> EventDetails);

public record EventDetails(Guid Id, string Title, DateOnly EventDate, Coordinates Location, decimal Duration, DateTime LastModifiedDate);

