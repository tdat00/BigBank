using System;
using System.Collections.Generic;
using System.Linq;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Sdk;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Service
{
    public class ExchangeRateHistoryService : IExchangeRateHistoryService
    {
        public ICurrencyService CurrencyService {get; set;}
        public IExchangeRateHistoryRepository ExchangeRateHistoryRepository {get; set;}
        public IUnitOfWork UnitOfWork {get; set;}
        public ILogger<IExchangeRateHistoryService> Logger {get; set;}

        public ExchangeRateHistoryService(
            ICurrencyService currencyService,

            IExchangeRateHistoryRepository exchangeRateHistoryRepository,

            IUnitOfWork unitOfWork,
            ILogger<IExchangeRateHistoryService> logger)
        {
            this.CurrencyService = currencyService;

            this.ExchangeRateHistoryRepository = exchangeRateHistoryRepository;

            this.UnitOfWork = unitOfWork;
            this.Logger = logger;
        }

        public decimal GetExchangeRate(string fromCurrency, string toCurrency, DateTime time)
            => fromCurrency.IsOrdinalEqual(toCurrency, true) ? 1m : GetExchangeRate(CurrencyService.Get(fromCurrency), CurrencyService.Get(toCurrency), time);

        public decimal GetExchangeRate(Currency from, Currency to, DateTime timeUtc)
        {
            if (from.Id == to.Id)
            {
                return 1m;
            }

            var exchangeRateAtTime = ExchangeRateHistoryRepository.All.FirstOrDefault(x => x.FromCurrencyId == from.Id && x.ToCurrencyId == to.Id && x.DateTimeUtc <= timeUtc);
            var exchangeRateRevertedAtTime = ExchangeRateHistoryRepository.All.FirstOrDefault(x => x.FromCurrencyId == to.Id && x.ToCurrencyId == from.Id && x.DateTimeUtc <= timeUtc);
            if (exchangeRateAtTime == null && exchangeRateRevertedAtTime == null)
            {
                throw new BusinessException("Cannot get exchange rate at that time.");
            }

            var rate = exchangeRateAtTime != null ? exchangeRateAtTime.Rate : (1m / exchangeRateRevertedAtTime.Rate);
            return rate;
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
            var fromCurrencyEntity = CurrencyService.Get(fromCurrency);
            var toCurrencyEntity = CurrencyService.Get(toCurrency);

            if (rate <= 0)
            {
                throw new BusinessException("Rate should be greater than 0.");
            }

            var entity = new ExchangeRateHistory
            {
                DateTimeUtc = time.ToUniversalTime(),
                FromCurrency = fromCurrencyEntity,
                ToCurrency = toCurrencyEntity,
                Rate = rate
            };

            ExchangeRateHistoryRepository.Create(entity);
            return entity;
        }
    }
}
