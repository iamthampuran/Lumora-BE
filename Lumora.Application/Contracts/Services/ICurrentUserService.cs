using Lumora.Application.Contracts.Common;

namespace Lumora.Application.Contracts.Services;

public interface ICurrentUserService
{
    UserTokenDetails? GetCurrentUserDetails();
    string GetCaller();
}
