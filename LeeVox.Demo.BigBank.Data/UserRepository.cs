using System.Linq;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public class UserRepository : BaseRepository<User>, IUserRepository, IRepository<User>
    {
        public UserRepository(IBigBankDbContext custoMerDbContext, ILogger<UserRepository> logger)
            : base(custoMerDbContext, logger)
        {
        }
    }
}
