using Ardalis.Result;
using Lumora.Application.Contracts.Common;
using Lumora.Application.Contracts.Persistence;
using Lumora.Application.Contracts.Services;

namespace Lumora.Application.Features.Auth.Queries.GetConsumerData;

public class GetConsumerDataQueryHandler(IUserRepository userRepository, IMinioService minioService, IUnitOfWork unitOfWork)
{
    public async Task<Result<GetConsumerDataResponse>> Handle(GetConsumerDataQuery query, CancellationToken cancellationToken)
    {
        var userDetails = unitOfWork.GetCurrentUserDetails();
        if (userDetails == null)
            return Result.Unauthorized("User not authorized");

        if (userDetails.ConsumerId == null)
            return Result.Unauthorized("User is not a consumer role user");

        var userData = await userRepository.GetFirstAsync(u => u.Id == userDetails.UserId, null, [u => u.ConsumerProfile], true, cancellationToken);
        if (userData == null) return Result.NotFound("User not found");

        var response = new GetConsumerDataResponse(userDetails.AvatarOrLogoUrl, userDetails.Name, userDetails.Email, userData.ConsumerProfile.Phone, userData.ConsumerProfile.Bio, userData.IsTwoFactorEnabled);
        return Result.Success(response);
    }
}
