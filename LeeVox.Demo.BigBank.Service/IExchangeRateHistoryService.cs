using System;
using System.Collections.Generic;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public interface IExchangeRateHistoryService : IService
    {
        decimal GetExchangeRate(string fromCurrency, string toCurrency, DateTime time);

        int InsertExchangeRate(DateTime time, string fromCurrency, string toCurrency, decimal rate);
        void InsertExchangeRate(IEnumerable<(DateTime time, string fromCurrency, string toCurrency, decimal rate)> exchangeRates);
    }
}
