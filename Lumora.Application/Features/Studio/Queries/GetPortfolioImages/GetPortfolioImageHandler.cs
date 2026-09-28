using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Studio.Queries.GetPortfolioImages;

public class GetPortfolioImageHandler(
    ILogger<GetPortfolioImageHandler> logger,
    IGenericRepository<Domain.Entities.Studio.PortfolioImage> portfolioRepository,
    ICurrentUserService currentUserService,
    IMinioService minioService)
{
    public async Task<Result<IEnumerable<GetPortoflioImageResponse>>> Handle(
        GetPorftolioImageQuery query,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handling query - {QueryName}",
            nameof(GetPorftolioImageQuery));

        var userDetails = currentUserService.GetCurrentUserDetails();

        if (userDetails is null)
            return Result.Unauthorized("User is not logged in");

        if (userDetails.StudioId is null)
            return Result.Forbidden(
                "User does not have a studio associated with their account");

        var portfolioImages = await portfolioRepository.GetAsync(
            p => p.StudioId == userDetails.StudioId.Value,
            null,
            null,
            true,
            false,
            cancellationToken);

        if (portfolioImages is null || !portfolioImages.Any())
            return Result.Success(
                Enumerable.Empty<GetPortoflioImageResponse>());

        var response = await Task.WhenAll(
            portfolioImages
                .OrderBy(p => p.DisplayOrder)
                .Select(async p =>
                    new GetPortoflioImageResponse(
                        p.Id,
                        await minioService.GeneratePresignedUrlAsync(p.ImageUrl),
                        p.Title,
                        p.DisplayOrder))
        );

        return Result.Success<IEnumerable<GetPortoflioImageResponse>>(response);
    }
}
