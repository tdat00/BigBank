using System.Linq;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Sdk;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Data
{
    public class CurrencyRepository : BaseRepository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(IBigBankDbContext dbContext, ILogger<ICurrencyRepository> logger)
            : base(dbContext, logger)
        {
        }

        public Currency ByName(string name)
        {
            return this.All.FirstOrDefault(x => name.IsOrdinalEqual(x.Name, true));
        }
    }
}
