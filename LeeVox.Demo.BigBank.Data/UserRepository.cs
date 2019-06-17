using System.Linq;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Data
{
    public class UserRepository : BaseRepository<User>, IUserRepository, IRepository<User>
    {
        public UserRepository(IBigBankDbContext custoMerDbContext)
            : base(custoMerDbContext)
        {
        }
    }
}
