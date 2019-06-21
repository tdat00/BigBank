using System;
using System.Collections.Generic;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Service
{
    public class ExchangeRateHistoryService : IExchangeRateHistoryService
    {
        public ICurrencyRepository CurrencyRepository {get; set;}
        public IExchangeRateHistoryRepository ExchangeRateHistoryRepository {get; set;}
        public IUnitOfWork UnitOfWork {get; set;}
        public ILogger<IExchangeRateHistoryService> Logger {get; set;}

        public ExchangeRateHistoryService(ICurrencyRepository currencyRepository, IExchangeRateHistoryRepository exchangeRateHistoryRepository, IUnitOfWork unitOfWork, ILogger<IExchangeRateHistoryService> logger)
        {
            this.CurrencyRepository = currencyRepository;
            this.ExchangeRateHistoryRepository = exchangeRateHistoryRepository;
            this.UnitOfWork = unitOfWork;
            this.Logger = logger;
        }

        public int InsertExchangeRate(DateTime time, string fromCurrency, string toCurrency, decimal rate)
        {
            var entity = _InsertExchangeRate(time, fromCurrency, toCurrency, rate);
            UnitOfWork.SaveChanges();

            return entity.Id;
        }

        public void InsertExchangeRate(IEnumerable<(DateTime, string, string, decimal)> exchangeRates)
        {
            //TODO: fix this N+1 issue
            foreach (var (time, fromCurrency, toCurrency, rate) in exchangeRates)
            {
                _InsertExchangeRate(time, fromCurrency, toCurrency, rate);
            }
            UnitOfWork.SaveChanges();
        }

        private ExchangeRateHistory _InsertExchangeRate(DateTime time, string fromCurrency, string toCurrency, decimal rate)
        {
            fromCurrency.EnsureNotNullOrWhiteSpace(nameof(fromCurrency));
            toCurrency.EnsureNotNullOrWhiteSpace(nameof(toCurrency));

            if (rate <= 0)
            {
                throw new BusinessException("Rate should be greater than 0.");
            }

            var timeUtc = time.ToUniversalTime();
            var fromCurrencyEntity = CurrencyRepository.ByName(fromCurrency);
            var toCurrencyEntity = CurrencyRepository.ByName(toCurrency);

            if (fromCurrencyEntity == null || toCurrencyEntity == null)
            {
                throw new BusinessException("Currency does not exist.");
            }

            var entity = new ExchangeRateHistory
            {
                DateTimeUtc = timeUtc,
                FromCurrency = fromCurrencyEntity,
                ToCurrency = toCurrencyEntity,
                Rate = rate
            };

            ExchangeRateHistoryRepository.Create(entity);
            return entity;
        }
    }
}
