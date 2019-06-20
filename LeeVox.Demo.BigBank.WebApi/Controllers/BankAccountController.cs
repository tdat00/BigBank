using System;
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
        
        [Route("{account}")]
        public ActionResult CheckBalance([FromRoute] string account)
        {
            try
            {
                var accountEntity = BankAccountService.Get(account);
                Logger.LogError(CurrentLoginInfo.User.Id + "++"+CurrentLoginInfo.User.FirstName + "++" + CurrentLoginInfo.User.LastName + "++" + CurrentLoginInfo.User.Email);
                Logger.LogError(accountEntity.UserId + "!" + accountEntity.AccountNumber);
                if (CurrentLoginInfo.User.Id != accountEntity.UserId)
                {
                    return Unauthorized();
                }

                return Ok(new {balance = accountEntity.Balance});
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

        public ActionResult DepositMoney(dynamic body)
        {
            throw new System.NotImplementedException();
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
