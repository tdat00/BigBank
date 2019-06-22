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
        public ILogger<IUserController> Logger {get; set;}

        public BankAccountController(CurrentLoginInfo currentLoginInfo, IUserService userService, IBankAccountService bankAccountService, ILogger<IUserController> logger)
        {
            this.CurrentLoginInfo = currentLoginInfo;
            this.UserService = userService;
            this.BankAccountService = bankAccountService;
            this.Logger = logger;
        }
        
        [Route("check-balance/{account?}")]
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
                    if (CurrentLoginInfo.User.Id != accountEntity.UserId)
                    {
                        return Unauthorized();
                    }
                    accounts.Add(accountEntity);
                }

                return Ok(accounts.Select(x => new {
                    account = x.AccountNumber,
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

        [Route("deposit")]
        [Authorize(Roles = "BankOfficer")]
        public ActionResult DepositMoney(dynamic body)
        {
            try
            {
                string account = body.account ?? body.Account ?? body.account_name ?? body.accountName ?? body.AccountName;
                string currency = body.currency ?? body.Currency;
                decimal amount = body.amount ?? body.Amount ?? body.money ?? body.Money;
                string message = body.message ?? body.Message;

                var id = BankAccountService.Deposit(CurrentLoginInfo.User, account, currency, amount, message);
                return Ok(new {id = id});
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

        public ActionResult QueryTransactions(dynamic body)
        {
            throw new System.NotImplementedException();
        }

        public ActionResult TransferMoney(dynamic body)
        {
            throw new System.NotImplementedException();
        }
    }
}
