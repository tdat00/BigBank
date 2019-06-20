using System.Linq;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Sdk;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IBigBankDbContext dbContext, ILogger<IUserRepository> logger)
            : base(dbContext, logger)
        {
        }

        public User ByEmail(string email)
        {
            return this.All.FirstOrDefault(x => email.IsOrdinalEqual(x.Email, true));
        }
    }
}
