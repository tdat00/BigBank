using System.Linq;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Sdk;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public class BankAccountRepository : BaseRepository<BankAccount>, IBankAccountRepository
    {
        public override IQueryable<BankAccount> All
        {
            get => base.All.IncludeProperty(x => x.Currency);
        }

        public BankAccountRepository(IBigBankDbContext dbContext, ILogger<IBankAccountRepository> logger)
            : base(dbContext, logger)
        {
        }

        public BankAccount ByName(string accountName)
        {
            return All.FirstOrDefault(x => accountName.IsOrdinalEqual(x.AccountNumber));
        }

        public IQueryable<BankAccount> ByUser(int userId)
        {
            return All.Where(x => x.UserId == userId);
        }
    }
}
