using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(IBigBankDbContext dbContext, ILogger<ITransactionRepository> logger)
            : base(dbContext, logger)
        {
        }
    }
}
