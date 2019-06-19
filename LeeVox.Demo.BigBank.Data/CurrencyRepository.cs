using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public class CurrencyRepository : BaseRepository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(IBigBankDbContext dbContext, ILogger<ICurrencyRepository> logger)
            : base(dbContext, logger)
        {
        }
    }
}
