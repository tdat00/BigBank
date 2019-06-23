using System.Collections.Generic;
using System.Linq;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Sdk;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Service
{
    public class CurrencyService : ICurrencyService
    {
        public ICurrencyRepository CurrencyRepository {get; set;}
        public IUnitOfWork UnitOfWork {get; set;}
        public ILogger<ICurrencyService> Logger {get; set;}

        public CurrencyService(
            ICurrencyRepository currencyRepository,

            IUnitOfWork unitOfWork,
            ILogger<ICurrencyService> logger
        )
        {
            this.CurrencyRepository = currencyRepository;

            this.UnitOfWork = unitOfWork;
            this.Logger = logger;
        }

        public Currency Get(int id)
        {
            var entity = CurrencyRepository.ById(id);
            entity.EnsureNotNull(message: $"Currency Id {id} does not exist.");

            return entity;
        }

        public Currency Get(string name)
        {
            name.EnsureNotNullOrWhiteSpace(nameof(name));
            
            var entity = CurrencyRepository.All.FirstOrDefault(x => name.IsOrdinalEqual(x.Name, true));
            entity.EnsureNotNull(message: $"Currency {name} does not exist.");

            return entity;
        }
    }
}
