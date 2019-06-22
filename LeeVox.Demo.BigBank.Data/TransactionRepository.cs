using System.Linq;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public class TransactionRepository : BaseRepository<Transaction>, IRepository<Transaction>
    {
        public TransactionRepository(IBigBankDbContext dbContext, ILogger<TransactionRepository> logger)
            : base(dbContext, logger)
        {
        }
    }
}
