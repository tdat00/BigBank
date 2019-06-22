using System;
using System.Collections.Generic;
using System.Linq;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Model;
using LeeVox.Demo.BigBank.Service;
using LeeVox.Sdk;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.WebApi.Controllers
{
    [Route("api/bank-account")]
    public class BankAccountController : BaseAuthenticatedApiController, IBankAccountController
    {
        public IUserService UserService {get; set;}
        public IBankAccountService BankAccountService {get; set;}
        public ITransactionService TransactionService {get; set;}
        public ICurrencyService CurrencyService {get; set;}
        public ILogger<IUserController> Logger {get; set;}

        public BankAccountController(CurrentLoginInfo currentLoginInfo, IUserService userService, IBankAccountService bankAccountService, ITransactionService transactionService, ICurrencyService currencyService, ILogger<IUserController> logger)
        {
            this.CurrentLoginInfo = currentLoginInfo;
            this.UserService = userService;
            this.BankAccountService = bankAccountService;
            this.TransactionService = transactionService;
            this.CurrencyService = currencyService;
            this.Logger = logger;
        }
        
        [HttpGet("check-balance/{account?}")]
        [Authorize(Roles = "BankOfficer, Customer")]
        public ActionResult CheckBalance([FromRoute] string account)
        {
            try
            {
                var accounts = new List<BankAccount>();
                if (string.IsNullOrWhiteSpace(account))
                {
                    accounts.AddRange(BankAccountService.GetByUser(CurrentLoginInfo.User.Id));
                }
                else
                {
                    var accountEntity = BankAccountService.Get(account);
                    if (CurrentLoginInfo.User.Role.HasFlag(UserRole.Customer) && !BankAccountService.IsOwnedByUser(account, CurrentLoginInfo.User))
                    {
                        return Forbid();
                    }
                    accounts.Add(accountEntity);
                }

                return Ok(accounts.Select(x => new {
                    account = x.AccountName,
                    currency = x.Currency.Name,
                    balance = x.Balance
                }));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is BusinessException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while checking balance: " + ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("query/{account}")]
        [Authorize(Roles = "BankOfficer, Customer")]
        public ActionResult QueryTransactions([FromRoute] string account, dynamic body)
        {
            try
            {
                if (CurrentLoginInfo.User.Role.HasFlag(UserRole.Customer) && !BankAccountService.IsOwnedByUser(account, CurrentLoginInfo.User))
                {
                    return Forbid();
                }

                string from = body.from ?? body.From ?? body.fromDate ?? body.FromDate;
                string to = body.to ?? body.To ?? body.toDate ?? body.ToDate;

                var (actualFrom, actualTo, transactions) = TransactionService.QueryTransactions(account, from, to);
                //TODO: move this to ViewModel converter.
                var result = transactions.Select(transaction =>
                {
                    string type = "unknown";
                    decimal? money = null;
                    Currency currency = null;
                    BankAccount sourceAccount = null;

                    switch (transaction)
                    {
                        case DepositMoneyTransaction depositTransaction:
                            type = "deposit";
                            money = depositTransaction.Money;
                            currency = depositTransaction.Currency;

                            break;
                        case WithdrawMoneyTransaction withdrawTransaction:
                            type = "withdraw";
                            money = withdrawTransaction.Money;
                            currency = withdrawTransaction.Currency;

                            break;
                        case TransferMoneyTransaction transferTransaction:
                            type = "transfer";
                            money = transferTransaction.Money;
                            currency = transferTransaction.Currency;
                            sourceAccount = transferTransaction.SourceAccount;

                            break;
                        default:
                            Logger.LogError("Should not come here. TransactionType is " + transaction.GetType());

                            break;
                    }

                    return new
                    {
                        type = type,
                        time = transaction.DateTimeUtc,
                        currency = currency?.Name,
                        money = money,
                        source = sourceAccount?.AccountName,
                        message = transaction.Message
                    };
                });
                return Ok(new {from = actualFrom, to = actualTo, transactions = result});
            }
            catch (Exception ex) when (ex is ArgumentException || ex is BusinessException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while checking balance: " + ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("deposit/{account}")]
        [Authorize(Roles = "BankOfficer")]
        public ActionResult DepositMoney([FromRoute] string account, dynamic body)
        {
            try
            {
                string currency = body.currency ?? body.Currency;
                decimal money = body.money ?? body.Money ?? body.amount ?? body.Amount;
                string message = body.message ?? body.Message ?? body.note ?? body.Note;

                var (transactionId, exchangeRate, delta, newBalance) = TransactionService.Deposit(CurrentLoginInfo.User, account, currency, money, message);
                return Ok(new {transaction = transactionId, rate = exchangeRate, delta = delta, balance = newBalance});
            }
            catch (Exception ex) when (ex is ArgumentException || ex is BusinessException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while checking balance: " + ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("withdraw/{account}")]
        [Authorize(Roles = "Customer")]
        public ActionResult WithdrawMoney([FromRoute] string account, dynamic body)
        {
            try
            {
                if (!BankAccountService.IsOwnedByUser(account, CurrentLoginInfo.User))
                {
                    return Forbid();
                }

                string currency = body.currency ?? body.Currency;
                decimal money = body.money ?? body.Money ?? body.amount ?? body.Amount;
                string message = body.message ?? body.Message ?? body.note ?? body.Note;

                var (transactionId, exchangeRate, delta, newBalance) = TransactionService.Withdraw(account, currency, money, message);
                return Ok(new {transaction = transactionId, rate = exchangeRate, delta = delta, balance = newBalance});
            }
            catch (Exception ex) when (ex is ArgumentException || ex is BusinessException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while checking balance: " + ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("transfer/{account}")]
        [Authorize(Roles = "Customer")]
        public ActionResult TransferMoney([FromRoute] string account, dynamic body)
        {
            try
            {
                if (!BankAccountService.IsOwnedByUser(account, CurrentLoginInfo.User))
                {
                    return Forbid();
                }

                string toAccount = body.to ?? body.To ?? body.toAccount ?? body.ToAccount;
                string currency = body.currency ?? body.Currency;
                decimal money = body.money ?? body.Money ?? body.amount ?? body.Amount;
                string message = body.message ?? body.Message ?? body.note ?? body.Note;

                var (transactionId, exchangeRateFromSource, deltaFromSource, newBalanceSource, exchangeRateToTarget, deltaToTarget, newBalanceTarget) = TransactionService.Transfer(account, toAccount, currency, money, message);
                return Ok(new
                {
                    transaction = transactionId,
                    exchange_rate_from_source_account = exchangeRateFromSource,
                    delta_from_source_account = deltaFromSource,
                    source_account_balance = newBalanceSource,
                    exchange_rate_to_target_account = exchangeRateToTarget,
                    delta_to_target_account = deltaToTarget,
                    target_account_balance = newBalanceTarget
                });
            }
            catch (Exception ex) when (ex is ArgumentException || ex is BusinessException)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while checking balance: " + ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
