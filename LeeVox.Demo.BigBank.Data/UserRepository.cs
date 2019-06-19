using System.Linq;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IBigBankDbContext custoMerDbContext, ILogger<IUserRepository> logger)
            : base(custoMerDbContext, logger)
        {
        }
    }
}
