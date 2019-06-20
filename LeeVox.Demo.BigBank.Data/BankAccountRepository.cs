using System.Linq;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Sdk;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public class BankAccountRepository : BaseRepository<BankAccount>, IBankAccountRepository
    {
        public BankAccountRepository(IBigBankDbContext dbContext, ILogger<IBankAccountRepository> logger)
            : base(dbContext, logger)
        {
        }

        public BankAccount ByAccountNumber(string accountNumber)
        {
            return All.FirstOrDefault(x => accountNumber.IsOrdinalEqual(x.AccountNumber, true));
        }
    }
}
