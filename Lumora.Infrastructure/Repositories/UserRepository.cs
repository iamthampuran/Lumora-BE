using Lumora.Application.Contracts.Persistence;
using Lumora.Domain.Entities.Identity;
using Lumora.Infrastructure.Data;

namespace Lumora.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    protected new readonly AppDbContext _appDbContext;
    public UserRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        _appDbContext = appDbContext;
    }
}
