using Lumora.Application.Helpers;
using Lumora.Domain.Enums;

namespace Lumora.Application.Features.Consumer.Queries.GetDashboardTable;

public record GetDashboardTableQuery(Guid Id, EventStatus Status, PaginationOptions PaginationOptions);
