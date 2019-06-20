using System;
using System.Collections.Generic;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Demo.BigBank.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    [Route("api/exchange-rate")]
    public class ExchangeRateController : BaseAuthenticatedApiController, IExchangeRateController
    {
        public IExchangeRateHistoryService ExchangeRateHistoryService {get; set;}
        public ILogger<IExchangeRateController> Logger {get; set;}
        public ExchangeRateController(CurrentLoginInfo currentLoginInfo, IExchangeRateHistoryService exchangeRateHistoryService, ILogger<IExchangeRateController> logger)
        {
            this.CurrentLoginInfo = currentLoginInfo;
            this.ExchangeRateHistoryService = exchangeRateHistoryService;
            this.Logger = logger;
        }

        [HttpPut]
        public ActionResult Insert([FromBody] dynamic body)
        {
            try
            {
                var rates = new List<(DateTime, string, string, decimal)>();
                var exchangeRates = body as JArray;
                foreach (dynamic exchangeRate in exchangeRates)
                {
                    DateTime time = exchangeRate.time ?? exchangeRate.date ?? exchangeRate.datetime;
                    string fromCurrency = exchangeRate.from_currency ?? exchangeRate.fromCurrency ?? exchangeRate.from;
                    string toCurrency = exchangeRate.to_currency ?? exchangeRate.toCurrency ?? exchangeRate.to;
                    decimal rate = exchangeRate.rate;

                    rates.Add((time, fromCurrency, toCurrency, rate));
                }
                
                ExchangeRateHistoryService.InsertExchangeRate(rates);
                return Ok();
            }
            catch (Exception ex) when (ex is ArgumentException || ex is BusinessException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while inserting new exchange rate: " + ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
