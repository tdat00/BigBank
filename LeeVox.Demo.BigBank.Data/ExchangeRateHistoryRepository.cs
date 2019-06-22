using System.Linq;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public class ExchangeRateHistoryRepository : BaseRepository<ExchangeRateHistory>, IExchangeRateHistoryRepository
    {
        public override IQueryable<ExchangeRateHistory> All
        {
            get => base.All.OrderByDescending(x => x.DateTimeUtc);
        }

        public ExchangeRateHistoryRepository(IBigBankDbContext dbContext, ILogger<IExchangeRateHistoryRepository> logger)
            : base(dbContext, logger)
        {
        }
    }
}
