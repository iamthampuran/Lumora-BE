using Lumora.Application.Helpers;
using Lumora.Domain.Enums;

namespace Lumora.Application.Features.Consumer.Queries.FindStudios;

public record FindStudiosQuery(Guid EventId, StudioFilterOptions? StudioFilter, PaginationOptions PaginationOptions, StudioSortOption SortOption = StudioSortOption.Recommended);
public record StudioFilterOptions(decimal? MaxDistance, decimal? MinRatings);
