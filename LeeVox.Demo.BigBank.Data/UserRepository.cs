using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IBigBankDbContext dbContext, ILogger<IUserRepository> logger)
            : base(dbContext, logger)
        {
        }
    }
}
