using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Lumora.Application.Features.Studio.Queries.GetStudioGalleries;
using Lumora.Application.Helpers;
using Lumora.Domain.Entities.Event;
using Lumora.Domain.Enums;
using Lumora.Infrastructure.Data;
using Lumora.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lumora.Infrastructure.Repositories;

public class GalleryRepository : GenericRepository<Gallery>, IGalleryRepository
{
    protected new readonly AppDbContext _appDbContext;
    private readonly IMinioService _minioService;

    public GalleryRepository(AppDbContext appDbContext, IMinioService minioService) : base(appDbContext)
    {
        _appDbContext = appDbContext;
        _minioService = minioService;
    }

    public async Task<PaginatedResponse<StudioGalleryDetails>> GetStudioGalleries(
     Guid studioId,
     GalleryStatus? status,
     PaginationOptions paginationOptions,
     string? searchText,
     CancellationToken cancellationToken)
    {
        var query = _appDbContext.Galleries
            .AsNoTracking()
            .Where(g => g.Inquiry.StudioId == studioId);

        if (status is not null)
        {
            query = query.Where(g => g.GalleryStatus == status);
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(g => g.GalleryName.Contains(searchText));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var galleries = await query
            .OrderByDescending(g => g.CreatedAt)
            .Skip((paginationOptions.PageCount - 1) * paginationOptions.PageSize)
            .Take(paginationOptions.PageSize)
            .Select(g => new
            {
                g.Id,
                g.GalleryName,
                g.GalleryStatus,
                g.GalleryCover,
                g.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var items = await Task.WhenAll(
            galleries.Select(async g =>
                new StudioGalleryDetails(
                    g.Id,
                    g.GalleryName,
                    g.GalleryStatus,
                    await _minioService.GeneratePresignedUrlAsync(g.GalleryCover),
                    DateOnly.FromDateTime(g.CreatedAt)
                )
            )
        );

        return new PaginatedResponse<StudioGalleryDetails>(
            [.. items],
            totalCount,
            paginationOptions.PageCount,
            paginationOptions.PageSize);
    }
}
