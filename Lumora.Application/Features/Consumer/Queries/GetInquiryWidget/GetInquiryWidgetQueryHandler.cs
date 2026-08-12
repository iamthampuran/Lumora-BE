using Ardalis.Result;
using Lumora.Application.Contracts.Persistence;
using Lumora.Domain.Entities.Identity;
using Microsoft.Extensions.Logging;

namespace Lumora.Application.Features.Consumer.Queries.GetInquiryWidget;

public class GetInquiryWidgetQueryHandler(IGenericRepository<ConsumerProfile> consumerRepository, IInquiryRepository inquiryRepository, ILogger<GetInquiryWidgetQueryHandler> logger)
{
    public async Task<Result<IEnumerable<GetInquiryWidgetResponse>>> Handle(GetInquiryWidgetQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling query -{@query}", nameof(GetInquiryWidgetQueryHandler));
        bool doesConsumerExist = await consumerRepository.AnyAsync(c => c.Id == query.ConsumerId);
        if (!doesConsumerExist)
        {
            return Result.NotFound("Consumer with the Id was not found");
        }
        var result = await inquiryRepository.GetInquiryWidgetDetailsAsync(query.ConsumerId, cancellationToken);
        return Result.Success(result);
    }
}
