using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public class BankAccountRepository : BaseRepository<BankAccount>, IBankAccountRepository
    {
        public BankAccountRepository(IBigBankDbContext dbContext, ILogger<IBankAccountRepository> logger)
            : base(dbContext, logger)
        {
        }
    }
}
