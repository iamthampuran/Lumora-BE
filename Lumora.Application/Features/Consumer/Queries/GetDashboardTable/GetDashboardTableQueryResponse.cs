using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Common.ValueObjects;
using Lumora.Domain.Enums;

namespace Lumora.Application.Features.Consumer.Queries.GetDashboardTable;


public record GetDashboardTableQueryResponse(int CreatedCount, int CompletedCount, int ActiveCount, PaginatedResponse<EventDetails> EventDetails);

public record EventDetails(Guid Id, string Title, DateOnly EventDate, string LocationName, decimal Duration, DateTime LastModifiedDate, string EventType, bool IsPredefined, decimal Budget);

